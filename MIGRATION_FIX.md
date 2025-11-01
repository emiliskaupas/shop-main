# PostgreSQL Migration Fix - Deployment Instructions

## Problem
The backend container was restarting because the migrations were created with SQL Server data types (`nvarchar`, `datetime2`) which are not compatible with PostgreSQL.

## Solution
1. ✅ Updated `ModelBuilderExtensions.cs` to explicitly use PostgreSQL-compatible types:
   - `varchar` instead of `nvarchar`
   - `text` instead of `nvarchar(max)`
   - PostgreSQL will automatically use `timestamp with time zone` for DateTime

2. ✅ Deleted old SQL Server migrations
3. ✅ Created new PostgreSQL-compatible migrations

## Deployment Steps

### On Your Local Machine (Windows)

1. **Commit the changes:**
   ```powershell
   git add .
   git commit -m "Fix: Replace SQL Server migrations with PostgreSQL-compatible migrations"
   git push origin master
   ```

### On Your DigitalOcean Droplet

1. **SSH into your droplet:**
   ```bash
   ssh root@<your-droplet-ip>
   ```

2. **Navigate to your project directory:**
   ```bash
   cd ~/shop-main
   ```

3. **Pull the latest changes:**
   ```bash
   git pull origin master
   ```

4. **Stop and remove the existing containers:**
   ```bash
   docker-compose down
   ```

5. **Remove the PostgreSQL volume to start with a clean database:**
   ```bash
   docker volume rm shop-main_postgres_data
   ```
   
   Or if the volume has a different name:
   ```bash
   docker volume ls  # List volumes
   docker volume rm <volume-name>  # Remove the postgres volume
   ```

6. **Rebuild the backend image:**
   ```bash
   docker-compose build --no-cache backend
   ```

7. **Start all containers:**
   ```bash
   docker-compose up -d
   ```

8. **Check the logs to verify it's working:**
   ```bash
   docker-compose logs -f backend
   ```

   You should see:
   ```
   Running database migrations...
   info: Microsoft.EntityFrameworkCore.Migrations[20402]
         Applying migration '20251101161701_InitialCreate'.
   Database migrations completed successfully!
   ```

9. **Verify all containers are running:**
   ```bash
   docker-compose ps
   ```

   All containers should show as "Up" (not "Restarting").

## Verification

Test the API health endpoint:
```bash
curl http://localhost:5000/api/health
```

Or from your browser:
```
http://<your-droplet-ip>/api/health
```

## If Issues Persist

1. **Check backend logs:**
   ```bash
   docker-compose logs backend
   ```

2. **Check PostgreSQL logs:**
   ```bash
   docker-compose logs postgres
   ```

3. **Connect to PostgreSQL to verify tables were created:**
   ```bash
   docker exec -it shop-postgres psql -U shopuser -d shopdb
   \dt  # List tables
   \q   # Quit
   ```

## Rollback (If Needed)

If something goes wrong, you can rollback:
```bash
docker-compose down
git checkout HEAD~1  # Go back to previous commit
docker-compose up -d
```
