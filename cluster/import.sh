#!/bin/bash

# Script to drop (--drop) or import (default) multiple MySQL and MongoDB databases in Docker containers

# === CONFIGURATION ===
MYSQL_CONTAINER="admin-mysql_db.1.yhrq2g4gbtxntksxvqyvnpzit"
MONGO_CONTAINER="admin-mongodb_mongo.1.irvcgjbviw6tiow8410l7zc2w"

DB_USER="root"
DB_PASSWORD="student"
MONGO_URI="mongodb://root:student@localhost:27017/?authSource=admin"

DUMP_DIR="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

MYSQL_DATABASES=(
  "RSWD_188597_offersdb"
  "RSWD_188597_authdb"
  "RSWD_188597_bookingsdb"
  "RSWD_188597_paymentsdb"
)

MONGO_DATABASE_NAMES=(
  "RSWD_188597_offersquerydb"
  "RSWD_188597_bookingseventsdb"
)

# === Determine Mode ===
IMPORT_MODE=true
if [[ "$1" == "--drop" ]]; then
    IMPORT_MODE=false
fi

mkdir -p "$DUMP_DIR"
sed -i 's/utf8mb4_uca1400_ai_ci/utf8mb4_general_ci/g' "$DUMP_DIR"/dump_*.sql 2>/dev/null

### === IMPORT MODE ===
if [ "$IMPORT_MODE" = true ]; then
    echo "🔄 === Import mode enabled ==="

    echo "🛠️  Creating and importing MySQL databases..."
    for DB_NAME in "${MYSQL_DATABASES[@]}"; do
        echo "🛠️  Processing MySQL database: $DB_NAME"
        docker exec "$MYSQL_CONTAINER" mysql -u"$DB_USER" -p"$DB_PASSWORD" -e "CREATE DATABASE IF NOT EXISTS \`$DB_NAME\`;"

        DUMP_FILE=$(ls -t "${DUMP_DIR}/dump_${DB_NAME}_"*.sql 2>/dev/null | head -n 1)
        if [ -z "$DUMP_FILE" ]; then
            echo "❌ No dump file found for $DB_NAME in $DUMP_DIR"
            exit 1
        fi

        echo "📥 Importing from $DUMP_FILE..."
        docker exec -i "$MYSQL_CONTAINER" mysql -u"$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" < "$DUMP_FILE"
        if [ $? -eq 0 ]; then
            echo "✅ Import completed for $DB_NAME"
        else
            echo "❌ Import failed for $DB_NAME"
            exit 1
        fi
    done

    echo "📥 Importing MongoDB databases..."
    for MONGO_DB in "${MONGO_DATABASE_NAMES[@]}"; do
        echo "🛠️  Processing MongoDB database: $MONGO_DB"

        MONGO_DUMP=$(ls -t "${DUMP_DIR}/mongo_${MONGO_DB}_"*.archive 2>/dev/null | head -n 1)
        if [ -z "$MONGO_DUMP" ]; then
            echo "❌ No MongoDB dump archive found for $MONGO_DB"
            exit 1
        fi

        echo "📥 Restoring from $MONGO_DUMP..."
        cat "$MONGO_DUMP" | docker exec -i "$MONGO_CONTAINER" mongorestore --uri="$MONGO_URI" --archive --nsFrom="${MONGO_DB}.*" --nsTo="${MONGO_DB}.*"
        if [ $? -eq 0 ]; then
            echo "✅ MongoDB import completed for $MONGO_DB"
        else
            echo "❌ MongoDB import failed for $MONGO_DB"
            exit 1
        fi
    done

### === DROP MODE ===
else
    echo "🗑️  === Drop mode enabled ==="

    echo "🗑️  Dropping MySQL databases..."
    for DB_NAME in "${MYSQL_DATABASES[@]}"; do
        echo "🗑️  Dropping MySQL database: $DB_NAME"
        docker exec "$MYSQL_CONTAINER" mysql -u"$DB_USER" -p"$DB_PASSWORD" -e "DROP DATABASE IF EXISTS \`$DB_NAME\`;"
        if [ $? -eq 0 ]; then
            echo "✅ Successfully dropped: $DB_NAME"
        else
            echo "❌ Failed to drop: $DB_NAME"
            exit 1
        fi
    done

    echo "🗑️  Dropping MongoDB databases..."
    for MONGO_DB in "${MONGO_DATABASE_NAMES[@]}"; do
        echo "🗑️  Dropping MongoDB database: $MONGO_DB"
        docker exec "$MONGO_CONTAINER" mongo "$MONGO_URI" --eval "db.getSiblingDB('$MONGO_DB').dropDatabase();"
        if [ $? -eq 0 ]; then
            echo "✅ Successfully dropped MongoDB: $MONGO_DB"
        else
            echo "❌ Failed to drop MongoDB: $MONGO_DB"
            exit 1
        fi
    done
fi
