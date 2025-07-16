#!/bin/bash

# Log cleanup script for PersonalFinanceTracker Enterprise Edition
# This script should be run daily via cron job

LOG_DIR="/var/log/personalfinancetracker"
RETENTION_DAYS=30
MAX_LOG_SIZE="50M"

echo "$(date): Starting log cleanup process..."

# Check if log directory exists
if [ ! -d "$LOG_DIR" ]; then
    echo "Log directory $LOG_DIR does not exist. Creating..."
    mkdir -p "$LOG_DIR"
    chmod 755 "$LOG_DIR"
fi

# Remove old log files (older than RETENTION_DAYS)
echo "Removing log files older than $RETENTION_DAYS days..."
find "$LOG_DIR" -name "*.log" -type f -mtime +$RETENTION_DAYS -delete

# Compress log files older than 7 days
echo "Compressing log files older than 7 days..."
find "$LOG_DIR" -name "*.log" -type f -mtime +7 -exec gzip {} \;

# Check log file sizes and rotate if necessary
for logfile in "$LOG_DIR"/*.log; do
    if [ -f "$logfile" ]; then
        size=$(du -h "$logfile" | cut -f1)
        echo "Checking $logfile (size: $size)"
        
        # If file is larger than MAX_LOG_SIZE, rotate it
        if [ "$(du -m "$logfile" | cut -f1)" -gt "$(echo $MAX_LOG_SIZE | sed 's/M//')" ]; then
            echo "Rotating large log file: $logfile"
            mv "$logfile" "$logfile.$(date +%Y%m%d-%H%M%S)"
            touch "$logfile"
        fi
    fi
done

# Clean up compressed files older than 90 days
echo "Removing compressed log files older than 90 days..."
find "$LOG_DIR" -name "*.log.gz" -type f -mtime +90 -delete

# Set proper permissions
chmod 644 "$LOG_DIR"/*.log 2>/dev/null || true

echo "$(date): Log cleanup process completed."
echo "Disk usage for log directory:"
du -sh "$LOG_DIR"

# Optional: Send summary to monitoring system
# curl -X POST "https://your-monitoring-endpoint/log-cleanup-summary" \
#      -H "Content-Type: application/json" \
#      -d "{\"timestamp\":\"$(date -u +%Y-%m-%dT%H:%M:%SZ)\",\"log_dir\":\"$LOG_DIR\",\"retention_days\":$RETENTION_DAYS}" 