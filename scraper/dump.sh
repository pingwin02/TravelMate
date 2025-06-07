#!/bin/bash

# Script to dump or import multiple MySQL and MongoDB databases from/to Docker containers

# === MySQL configuration ===
MYSQL_CONTAINER="admin-mysql_db"
MYSQL_USER="root"
MYSQL_PASSWORD="student"
MYSQL_DB_NAMES=($(grep -oP 'CREATE DATABASE \K\w+' ../init.sql))

# === MongoDB configuration ===
MONGO_CONTAINER="mongodb"
MONGO_URI="mongodb://root:student@localhost:27017/?authSource=admin"
MONGO_DATABASE_NAME="RSWD_188597_offersquerydb"

# === Common ===
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
DUMP_DIR="./backups"
IMPORT_MODE=false

if [[ "$1" == "--restore" ]]; then
    IMPORT_MODE=true
fi

mkdir -p "$DUMP_DIR"

### === IMPORT MODE ===
if [ "$IMPORT_MODE" = true ]; then
    echo "=== Import mode enabled ==="

    echo "🗑️ Dropping existing MySQL databases..."
    for DB_NAME in "${MYSQL_DB_NAMES[@]}"; do
        echo "🗑️ Dropping database: $DB_NAME..."
        docker exec "$MYSQL_CONTAINER" mariadb -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" -e "DROP DATABASE IF EXISTS \`$DB_NAME\`;"
        docker exec "$MYSQL_CONTAINER" mariadb -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" -e "CREATE DATABASE \`$DB_NAME\`;"
    done

    echo "📥 Importing MySQL databases..."
    for DB_NAME in "${MYSQL_DB_NAMES[@]}"; do
        DUMP_FILE=$(ls -t "${DUMP_DIR}/dump_${DB_NAME}_"*.sql 2>/dev/null | head -n 1)

        if [ -z "$DUMP_FILE" ]; then
            echo "❌ No dump file found for $DB_NAME in $DUMP_DIR"
            exit 1
        fi

        echo "✅ Importing $DB_NAME from $DUMP_FILE..."
        docker exec -i "$MYSQL_CONTAINER" mariadb -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" < "$DUMP_FILE"

        if [ $? -eq 0 ]; then
            echo "✅ Import successful: $DB_NAME"
        else
            echo "❌ Import failed for $DB_NAME"
            exit 1
        fi
    done

    echo "🗑️ Dropping MongoDB database before import..."
    docker exec "$MONGO_CONTAINER" mongosh "$MONGO_URI" --eval "db.getSiblingDB('$MONGO_DATABASE_NAME').dropDatabase()"

    echo "📥 Importing MongoDB database..."
    MONGO_ARCHIVE=$(ls -t "$DUMP_DIR/mongo_${MONGO_DATABASE_NAME}_"*.archive 2>/dev/null | head -n 1)

    if [ -z "$MONGO_ARCHIVE" ]; then
        echo "❌ No MongoDB archive found for $MONGO_DATABASE_NAME"
        exit 1
    fi

    cat "$MONGO_ARCHIVE" | docker exec -i "$MONGO_CONTAINER" mongorestore --uri="$MONGO_URI" --archive --nsFrom="${MONGO_DATABASE_NAME}.*" --nsTo="${MONGO_DATABASE_NAME}.*"

    if [ $? -eq 0 ]; then
        echo "✅ MongoDB restore successful"
    else
        echo "❌ MongoDB restore failed"
        exit 1
    fi

### === DUMP MODE ===
else
    echo "=== Dump mode enabled ==="

    echo "📤 Dumping MySQL databases..."
    for DB_NAME in "${MYSQL_DB_NAMES[@]}"; do
        DUMP_FILE="${DUMP_DIR}/dump_${DB_NAME}_${TIMESTAMP}.sql"

        echo "🔄 Dumping database: $DB_NAME..."
        docker exec "$MYSQL_CONTAINER" mariadb-dump -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" > "$DUMP_FILE"

        if [ $? -eq 0 ]; then
            echo "✅ Database dump successful: $DUMP_FILE"
        else
            echo "❌ Database dump failed for $DB_NAME"
            rm "$DUMP_FILE"
            exit 1
        fi
    done

    echo "📤 Dumping MongoDB database (all collections)..."
    MONGO_ARCHIVE="${DUMP_DIR}/mongo_${MONGO_DATABASE_NAME}_${TIMESTAMP}.archive"

    docker exec "$MONGO_CONTAINER" mongodump --uri="$MONGO_URI" --archive --db="$MONGO_DATABASE_NAME" > "$MONGO_ARCHIVE"

    if [ $? -eq 0 ]; then
        echo "✅ MongoDB dump successful: $MONGO_ARCHIVE"
    else
        echo "❌ MongoDB dump failed"
        rm "$MONGO_ARCHIVE"
        exit 1
    fi
fi
