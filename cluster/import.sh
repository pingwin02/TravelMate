#!/bin/bash

# Script to drop (--drop) or import (default) multiple MySQL and MongoDB databases in Docker containers

# === CONFIGURATION ===
MYSQL_CONTAINER="admin-mysql_db.1.yhrq2g4gbtxntksxvqyvnpzit"
MONGO_CONTAINER="admin-mongodb_mongo.1.irvcgjbviw6tiow8410l7zc2w"

DB_USER="root"
DB_PASSWORD="student"
MONGO_URI="mongodb://root:student@localhost:27017/?authSource=admin"
MONGO_DATABASE="RSWD_188597_offersquerydb"

DUMP_DIR="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

MYSQL_DATABASES=(
  "RSWD_188597_offersdb"
  "RSWD_188597_authdb"
  "RSWD_188597_bookingsdb"
  "RSWD_188597_paymentsdb"
)

# === Determine Mode ===
IMPORT_MODE=true
if [[ "$1" == "--drop" ]]; then
    IMPORT_MODE=false
fi

mkdir -p "$DUMP_DIR"
sed -i 's/utf8mb4_uca1400_ai_ci/utf8mb4_general_ci/g' "$DUMP_DIR"/dump_*.sql 2>/dev/null

if [ "$IMPORT_MODE" = true ]; then
    echo "=== Import mode enabled ==="

    echo "📥 Importing MySQL databases..."
    for DB_NAME in "${MYSQL_DATABASES[@]}"; do
        echo "🛠️ Creating database: $DB_NAME..."
        docker exec "$MYSQL_CONTAINER" mysql -u"$DB_USER" -p"$DB_PASSWORD" -e "CREATE DATABASE IF NOT EXISTS \`$DB_NAME\`;"

        DUMP_FILE=$(ls -t "${DUMP_DIR}/dump_${DB_NAME}_"*.sql 2>/dev/null | head -n 1)
        if [ -z "$DUMP_FILE" ]; then
            echo "❌ No dump file found for $DB_NAME in $DUMP_DIR"
            exit 1
        fi

        echo "📥 Importing $DB_NAME from $DUMP_FILE..."
        docker exec -i "$MYSQL_CONTAINER" mysql -u"$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" < "$DUMP_FILE"
        [[ $? -eq 0 ]] && echo "✅ Import successful: $DB_NAME" || { echo "❌ Import failed: $DB_NAME"; exit 1; }
    done

    echo "📥 Importing MongoDB database..."
    MONGO_DUMP=$(ls -t "${DUMP_DIR}/mongo_${MONGO_DATABASE}_"*.archive 2>/dev/null | head -n 1)
    if [ -z "$MONGO_DUMP" ]; then
        echo "❌ No MongoDB dump archive found"
        exit 1
    fi

    cat "$MONGO_DUMP" | docker exec -i "$MONGO_CONTAINER" mongorestore --uri="$MONGO_URI" --archive --nsFrom="${MONGO_DATABASE}.*" --nsTo="${MONGO_DATABASE}.*"
    [[ $? -eq 0 ]] && echo "✅ MongoDB import successful" || { echo "❌ MongoDB import failed"; exit 1; }

else
    echo "=== Drop mode enabled ==="

    echo "🗑️ Dropping MySQL databases..."
    for DB_NAME in "${MYSQL_DATABASES[@]}"; do
        echo "Dropping database: $DB_NAME..."
        docker exec "$MYSQL_CONTAINER" mysql -u"$DB_USER" -p"$DB_PASSWORD" -e "DROP DATABASE IF EXISTS \`$DB_NAME\`;"
        [[ $? -eq 0 ]] && echo "✅ Dropped: $DB_NAME" || { echo "❌ Failed to drop: $DB_NAME"; exit 1; }
    done

    echo "🗑️ Dropping MongoDB database..."
    docker exec "$MONGO_CONTAINER" mongo "$MONGO_URI" --eval "db.getSiblingDB('$MONGO_DATABASE').dropDatabase();"
    [[ $? -eq 0 ]] && echo "✅ MongoDB database dropped" || { echo "❌ Failed to drop MongoDB database"; exit 1; }
fi
