# 🚀 Complete DigitalOcean Deployment Guide

This guide will walk you through deploying your full-stack Shop application to DigitalOcean using Docker Compose.

## 📋 Table of Contents
1. [Get Free $200 Credit](#1-get-free-200-credit)
2. [Create a Droplet](#2-create-a-droplet)
3. [Prepare Your Code](#3-prepare-your-code)
4. [Deploy to DigitalOcean](#4-deploy-to-digitalocean)
5. [Configure Domain (Optional)](#5-configure-domain-optional)
6. [Troubleshooting](#troubleshooting)

---

## 1. Get Free $200 Credit

### Steps:
1. **Sign up for DigitalOcean**:
   - Go to: https://try.digitalocean.com/freetrialoffer/
   - Or use referral link: https://m.do.co/c/your-referral-code
   
2. **Verify your account**:
   - Add a credit card (won't be charged during trial)
   - Verify your email
   
3. **Receive $200 credit** valid for 60 days!

---

## 2. Create a Droplet

### Steps:

1. **Log into DigitalOcean** and click **"Create"** → **"Droplets"**

2. **Choose Image**:
   - Click on **"Marketplace"** tab
   - Search for **"Docker on Ubuntu"**
   - Select **"Docker on Ubuntu 22.04"** (Docker pre-installed!)

3. **Choose Plan**:
   - **Basic** plan
   - **Regular** CPU option
   - Select **$12/month** (2GB RAM, 1 CPU, 50GB SSD)
   - This is enough for your application

4. **Choose Datacenter Region**:
   - Pick closest to your users (e.g., New York, London, Frankfurt)

5. **Authentication**:
   - Choose **SSH Key** (recommended) or **Password**
   - If SSH Key: 
     - On Windows, generate with: `ssh-keygen -t rsa`
     - Copy from `C:\Users\YourName\.ssh\id_rsa.pub`
     - Paste into DigitalOcean

6. **Finalize**:
   - Give your droplet a name (e.g., "shop-production")
   - Click **"Create Droplet"**
   - Wait 1-2 minutes for creation

7. **Note your Droplet's IP address** (e.g., `142.93.xxx.xxx`)

---

## 3. Prepare Your Code

### A. Update Frontend API URL

Before deploying, update your frontend to use the correct API URL:

**File**: `frontend/src/config.ts` (create if doesn't exist)

```typescript
// API configuration
const API_BASE_URL = process.env.NODE_ENV === 'production' 
  ? '/api'  // In production, use relative URL (nginx proxy)
  : 'http://localhost:5000/api';

export { API_BASE_URL };
```

Then update your axios calls to use this URL.

### B. Create Environment File

On your **local machine**, in the project root:

```bash
cp .env.example .env
```

Edit `.env` and set secure values:

```env
DB_PASSWORD=YourSecurePassword123!
JWT_SECRET=generate-a-very-long-random-string-here-at-least-64-characters-long
```

**Generate secure JWT secret** (PowerShell):
```powershell
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | % {[char]$_})
```

### C. Push to GitHub

```bash
git add .
git commit -m "Add Docker deployment configuration"
git push origin master
```

---

## 4. Deploy to DigitalOcean

### A. Connect to Your Droplet

**Windows (PowerShell)**:
```powershell
ssh root@YOUR_DROPLET_IP
```

Example:
```powershell
ssh root@142.93.xxx.xxx
```

Type `yes` when asked about fingerprint.

### B. Install Required Tools

Once connected to your droplet:

```bash
# Update system
apt update && apt upgrade -y

# Install git (if not already installed)
apt install git -y

# Verify Docker is installed
docker --version
docker-compose --version
```

### C. Clone Your Repository

```bash
# Navigate to home directory
cd ~

# Clone your repository
git clone https://github.com/emiliskaupas/shop-main.git

# Navigate into project
cd shop-main
```

### D. Configure Environment Variables

```bash
# Create .env file
cp .env.example .env

# Edit with nano
nano .env
```

Update the values:
```env
DB_PASSWORD=YourVerySecurePassword123!
JWT_SECRET=your-64-character-random-string-here
```

**Save and exit**: Press `Ctrl+X`, then `Y`, then `Enter`

### E. Build and Start Application

```bash
# Make sure you're in the project directory
cd ~/shop-main

# Build and start all containers
docker-compose up -d --build
```

This will:
- Build your backend Docker image
- Build your frontend Docker image
- Pull PostgreSQL image
- Start all containers
- Create the database

**Expected output**:
```
Creating network "shop-main_shop-network" with driver "bridge"
Creating volume "shop-main_postgres_data" with local driver
Creating shop-postgres ... done
Creating shop-backend  ... done
Creating shop-frontend ... done
```

### F. Run Database Migrations

```bash
# Wait 30 seconds for containers to fully start
sleep 30

# Run migrations
docker-compose exec backend dotnet ef database update
```

### G. Verify Deployment

Check if all containers are running:
```bash
docker-compose ps
```

You should see:
```
NAME              STATUS          PORTS
shop-backend      Up 2 minutes    0.0.0.0:5000->5000/tcp
shop-frontend     Up 2 minutes    0.0.0.0:80->80/tcp, 0.0.0.0:443->443/tcp
shop-postgres     Up 2 minutes    0.0.0.0:5432->5432/tcp
```

### H. Test Your Application

**In your browser**, go to:
```
http://YOUR_DROPLET_IP
```

Example: `http://142.93.123.456`

You should see your React application! 🎉

---

## 5. Configure Domain (Optional)

### A. Point Domain to Droplet

1. **In your domain registrar** (Namecheap, GoDaddy, etc.):
   - Create an **A record**
   - Point to your Droplet IP
   - Example: `shop.yourdomain.com` → `142.93.xxx.xxx`

2. **Wait for DNS propagation** (5-60 minutes)

### B. Set Up SSL with Let's Encrypt

```bash
# Install Certbot
apt install certbot python3-certbot-nginx -y

# Stop frontend container temporarily
docker-compose stop frontend

# Get SSL certificate
certbot certonly --standalone -d yourdomain.com -d www.yourdomain.com

# Restart frontend
docker-compose start frontend
```

### C. Update Nginx for HTTPS

Update `frontend/nginx.conf` to include SSL configuration, then rebuild:

```bash
docker-compose up -d --build frontend
```

---

## 🔧 Useful Commands

### View Logs
```bash
# All containers
docker-compose logs -f

# Specific container
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f postgres
```

### Restart Application
```bash
docker-compose restart
```

### Stop Application
```bash
docker-compose down
```

### Update Application (Deploy New Changes)
```bash
# Pull latest code
git pull origin master

# Rebuild and restart
docker-compose down
docker-compose up -d --build

# Run new migrations if any
docker-compose exec backend dotnet ef database update
```

### Access Database
```bash
docker-compose exec postgres psql -U shopuser -d shopdb
```

### Check Container Resource Usage
```bash
docker stats
```

---

## 🐛 Troubleshooting

### Problem: Can't connect to application

**Check firewall**:
```bash
# Allow HTTP and HTTPS
ufw allow 80
ufw allow 443
ufw allow 22  # SSH - IMPORTANT!
ufw enable
```

### Problem: Backend won't start

**Check logs**:
```bash
docker-compose logs backend
```

**Common issues**:
- Database not ready → Wait longer, then restart: `docker-compose restart backend`
- Connection string wrong → Check `.env` file

### Problem: Frontend shows blank page

**Check logs**:
```bash
docker-compose logs frontend
```

**Rebuild frontend**:
```bash
docker-compose up -d --build frontend
```

### Problem: Database migration fails

**Manual migration**:
```bash
# Enter backend container
docker-compose exec backend bash

# Run migration
dotnet ef database update

# Exit
exit
```

### Problem: Out of memory

**Upgrade Droplet**:
- Go to DigitalOcean dashboard
- Click on your Droplet
- Click "Resize"
- Choose 4GB RAM plan

---

## 📊 Monitoring & Maintenance

### Monitor Logs
```bash
# Set up log rotation to prevent disk fill
apt install logrotate -y
```

### Automatic Backups

**Enable DigitalOcean Backups** (costs extra 20%):
1. Go to your Droplet settings
2. Enable "Backups"
3. Weekly automatic snapshots created

**Or Database Backups**:
```bash
# Backup database
docker-compose exec postgres pg_dump -U shopuser shopdb > backup-$(date +%Y%m%d).sql

# Restore database
docker-compose exec -T postgres psql -U shopuser shopdb < backup-20250101.sql
```

---

## 💰 Cost Estimate

- **Droplet (2GB RAM)**: $12/month
- **Bandwidth**: 2TB included (free)
- **Backups** (optional): +$2.40/month
- **Domain** (if buying): ~$10-15/year

**Total**: ~$12-15/month (or FREE with $200 credit for ~16 months!)

---

## 🎯 Next Steps

After deployment:
1. ✅ Set up automatic backups
2. ✅ Configure domain with SSL
3. ✅ Set up monitoring (DigitalOcean Monitoring is free)
4. ✅ Configure email notifications for alerts
5. ✅ Set up CI/CD with GitHub Actions (optional)

---

## 📞 Support

- **DigitalOcean Docs**: https://docs.digitalocean.com
- **Community**: https://www.digitalocean.com/community
- **Docker Docs**: https://docs.docker.com

---

**🎉 Congratulations! Your application is now live on DigitalOcean!** 🎉
