#!/bin/bash

# Script to drop (default) or import multiple databases in a Docker container

CONTAINER_NAME="admin-mysql_db.1.yhrq2g4gbtxntksxvqyvnpzit"
DB_USER="root"
DB_PASSWORD="student"
DUMP_DIR="./dumps"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

sed -i 's/utf8mb4_uca1400_ai_ci/utf8mb4_general_ci/g' "$DUMP_DIR"/dump_*.sql

DB_NAMES=(
  "RSWD_188597_offersdb"
  "RSWD_188597_authdb"
  "RSWD_188597_bookingsdb"
  "RSWD_188597_paymentsdb"
)

IMPORT_MODE=true
if [[ "$1" == "--drop" ]]; then
    IMPORT_MODE=false
fi

mkdir -p "$DUMP_DIR"

if [ "$IMPORT_MODE" = true ]; then
    echo "Import mode enabled. Creating and importing databases..."

    for DB_NAME in "${DB_NAMES[@]}"; do
        echo "Creating database: $DB_NAME..."
        docker exec "$CONTAINER_NAME" mysql -u"$DB_USER" -p"$DB_PASSWORD" -e "CREATE DATABASE IF NOT EXISTS \`$DB_NAME\`;"

        DUMP_FILE=$(ls -t "${DUMP_DIR}/dump_${DB_NAME}_"*.sql 2>/dev/null | head -n 1)
        if [ -z "$DUMP_FILE" ]; then
            echo "No dump file found for $DB_NAME in $DUMP_DIR"
            exit 1
        fi

        echo "Importing $DB_NAME from $DUMP_FILE..."
        docker exec -i "$CONTAINER_NAME" mysql -u"$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" < "$DUMP_FILE"

        if [ $? -eq 0 ]; then
            echo "Import successful: $DB_NAME"
        else
            echo "Import failed for $DB_NAME"
            exit 1
        fi
    done

else
    echo "Dropping databases..."
    for DB_NAME in "${DB_NAMES[@]}"; do
        echo "Dropping database: $DB_NAME..."
        docker exec "$CONTAINER_NAME" mysql -u"$DB_USER" -p"$DB_PASSWORD" -e "DROP DATABASE IF EXISTS \`$DB_NAME\`;"

        if [ $? -eq 0 ]; then
            echo "Dropped: $DB_NAME"
        else
            echo "Failed to drop: $DB_NAME"
            exit 1
        fi
    done
fi
