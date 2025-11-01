# Docker Compose Local Testing

## Test Locally Before Deploying

Before deploying to DigitalOcean, you can test the Docker setup locally.

### Prerequisites
- Docker Desktop installed on Windows
- WSL2 enabled (for Windows)

### Steps

1. **Install Docker Desktop**:
   - Download: https://www.docker.com/products/docker-desktop/
   - Install and restart computer
   - Start Docker Desktop

2. **Create .env file**:
   ```powershell
   cd C:\Users\Emilis\Desktop\shop-main\shop-main
   Copy-Item .env.example .env
   ```

3. **Edit .env** (optional - defaults work for local testing):
   ```env
   DB_PASSWORD=LocalPassword123!
   JWT_SECRET=local-dev-secret-key-for-testing-only
   ```

4. **Build and run**:
   ```powershell
   docker-compose up --build
   ```

5. **Wait for containers to start** (watch the logs)

6. **Run migrations** (in a new terminal):
   ```powershell
   docker-compose exec backend dotnet ef database update
   ```

7. **Open in browser**:
   - Frontend: http://localhost
   - Backend API: http://localhost:5000/api
   - Swagger: http://localhost:5000/swagger

8. **Stop containers**:
   ```powershell
   # Press Ctrl+C in the terminal running docker-compose
   # Or run:
   docker-compose down
   ```

### Troubleshooting Local Setup

**Port already in use?**
```powershell
# Find what's using port 80
netstat -ano | findstr :80

# Kill the process (replace PID with actual number)
taskkill /F /PID <PID>
```

**Docker not starting?**
- Make sure Docker Desktop is running
- Check WSL2 is enabled: `wsl --status`
- Restart Docker Desktop

**Database connection failed?**
```powershell
# Restart containers
docker-compose restart

# View logs
docker-compose logs postgres
docker-compose logs backend
```

### Clean Up

```powershell
# Stop and remove all containers
docker-compose down

# Remove volumes (deletes database data)
docker-compose down -v

# Remove images (free up space)
docker-compose down --rmi all
```

---

Once local testing works, proceed with DigitalOcean deployment!
