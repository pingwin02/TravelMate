#!/bin/bash

# Script to dump or import multiple MySQL and MongoDB databases from/to Docker containers

# === MySQL configuration ===
MYSQL_CONTAINER="admin-mysql_db"
MYSQL_USER="root"
MYSQL_PASSWORD="student"
MYSQL_DB_NAMES=($(grep -oP 'CREATE DATABASE \K\w+' ../init.sql))

# === MongoDB configuration ===
MONGO_CONTAINER="admin-mongodb_mongo"
MONGO_URI="mongodb://root:student@localhost:27017/?authSource=admin"
MONGO_DATABASE_NAMES=(
    "RSWD_188597_offersquerydb"
    "RSWD_188597_bookingseventsdb"
)

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
    echo "🔄 === Import mode enabled ==="

    echo "🗑️  Dropping and recreating MySQL databases..."
    for DB_NAME in "${MYSQL_DB_NAMES[@]}"; do
        echo "🗑️  Dropping database: $DB_NAME"
        docker exec "$MYSQL_CONTAINER" mariadb -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" -e "DROP DATABASE IF EXISTS \`$DB_NAME\`;"
        docker exec "$MYSQL_CONTAINER" mariadb -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" -e "CREATE DATABASE \`$DB_NAME\`;"
    done

    echo "📥 Importing MySQL dumps..."
    for DB_NAME in "${MYSQL_DB_NAMES[@]}"; do
        DUMP_FILE=$(ls -t "${DUMP_DIR}/dump_${DB_NAME}_"*.sql 2>/dev/null | head -n 1)

        if [ -z "$DUMP_FILE" ]; then
            echo "❌ No dump file found for $DB_NAME in $DUMP_DIR"
            exit 1
        fi

        echo "💾 Importing $DB_NAME from $DUMP_FILE..."
        docker exec -i "$MYSQL_CONTAINER" mariadb -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" < "$DUMP_FILE"

        if [ $? -eq 0 ]; then
            echo "✅ Import successful: $DB_NAME"
        else
            echo "❌ Import failed for $DB_NAME"
            exit 1
        fi
    done

    echo "📥 Restoring MongoDB databases..."
    for DB_NAME in "${MONGO_DATABASE_NAMES[@]}"; do
        echo "🗑️  Dropping MongoDB database: $DB_NAME"
        docker exec "$MONGO_CONTAINER" mongosh "$MONGO_URI" --eval "db.getSiblingDB('$DB_NAME').dropDatabase()"

        MONGO_ARCHIVE=$(ls -t "$DUMP_DIR/mongo_${DB_NAME}_"*.archive 2>/dev/null | head -n 1)

        if [ -z "$MONGO_ARCHIVE" ]; then
            echo "❌ No MongoDB archive found for $DB_NAME"
            exit 1
        fi

        echo "💾 Restoring $DB_NAME from $DUMP_FILE..."
        cat "$MONGO_ARCHIVE" | docker exec -i "$MONGO_CONTAINER" mongorestore --uri="$MONGO_URI" --archive --nsFrom="${DB_NAME}.*" --nsTo="${DB_NAME}.*"

        if [ $? -eq 0 ]; then
            echo "✅ MongoDB restore successful: $DB_NAME"
        else
            echo "❌ MongoDB restore failed for $DB_NAME"
            exit 1
        fi
    done

### === DUMP MODE ===
else
    echo "📦 === Dump mode enabled ==="

    echo "💾 Dumping MySQL databases..."
    for DB_NAME in "${MYSQL_DB_NAMES[@]}"; do
        echo "📤 Dumping database: $DB_NAME"
        DUMP_FILE="${DUMP_DIR}/dump_${DB_NAME}_${TIMESTAMP}.sql"

        docker exec "$MYSQL_CONTAINER" mariadb-dump -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" > "$DUMP_FILE"

        if [ $? -eq 0 ]; then
            echo "✅ MySQL dump successful: $DUMP_FILE"
        else
            echo "❌ MySQL dump failed for $DB_NAME"
            rm "$DUMP_FILE"
            exit 1
        fi
    done

    echo "💾 Dumping MongoDB databases..."
    for DB_NAME in "${MONGO_DATABASE_NAMES[@]}"; do
        echo "📤 Dumping MongoDB database: $DB_NAME"
        MONGO_ARCHIVE="${DUMP_DIR}/mongo_${DB_NAME}_${TIMESTAMP}.archive"

        docker exec "$MONGO_CONTAINER" mongodump --uri="$MONGO_URI" --archive --db="$DB_NAME" > "$MONGO_ARCHIVE"

        if [ $? -eq 0 ]; then
            echo "✅ MongoDB dump successful: $MONGO_ARCHIVE"
        else
            echo "❌ MongoDB dump failed for $DB_NAME"
            rm "$MONGO_ARCHIVE"
            exit 1
        fi
    done
fi
