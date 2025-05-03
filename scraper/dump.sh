#!/bin/bash

# Script to dump or import multiple databases from/to a Docker container

CONTAINER_NAME="db"
DB_USER="root"
DB_PASSWORD="student"
DB_NAMES=("RSWD_188597_offersdb" "RSWD_188597_authdb" "RSWD_188597_bookingsdb")
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
DUMP_DIR="./dumps"

IMPORT_MODE=false
if [[ "$1" == "--restore" ]]; then
    IMPORT_MODE=true
fi

mkdir -p "$DUMP_DIR"

if [ "$IMPORT_MODE" = true ]; then
    echo "Import mode enabled. Importing databases from dump files..."
    for DB_NAME in "${DB_NAMES[@]}"; do
        DUMP_FILE=$(ls -t "${DUMP_DIR}/dump_${DB_NAME}_"*.sql 2>/dev/null | head -n 1)

        if [ -z "$DUMP_FILE" ]; then
            echo "❌ No dump file found for $DB_NAME in $DUMP_DIR"
            exit 1
        fi

        echo "✅ Importing $DB_NAME from $DUMP_FILE..."
        docker exec -i "$CONTAINER_NAME" mariadb -u"$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" < "$DUMP_FILE"

        if [ $? -eq 0 ]; then
            echo "✅ Import successful: $DB_NAME"
        else
            echo "❌ Import failed for $DB_NAME"
            exit 1
        fi
    done
else
    echo "Dump mode. Dumping databases..."
    for DB_NAME in "${DB_NAMES[@]}"; do
        DUMP_FILE="${DUMP_DIR}/dump_${DB_NAME}_${TIMESTAMP}.sql"

        echo "🔄 Dumping database: $DB_NAME..."
        docker exec "$CONTAINER_NAME" mariadb-dump -u"$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" > "$DUMP_FILE"

        if [ $? -eq 0 ]; then
            echo "✅ Database dump successful: $DUMP_FILE"
        else
            echo "❌ Database dump failed for $DB_NAME"
            rm "$DUMP_FILE"
            exit 1
        fi
    done
fi
