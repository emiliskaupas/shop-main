# 🚀 Quick Deployment Cheat Sheet

## Prerequisites
- DigitalOcean account with $200 credit
- GitHub repository
- 10 minutes

---

## Step-by-Step (Copy & Paste Ready)

### 1️⃣ Create Droplet
1. Go to: https://cloud.digitalocean.com/droplets/new
2. Choose: **"Docker on Ubuntu 22.04"** from Marketplace
3. Size: **$12/month (2GB RAM)**
4. Add SSH key or use password
5. Click **"Create Droplet"**
6. Copy your IP: `XXX.XXX.XXX.XXX`

---

### 2️⃣ Connect to Droplet

**Windows PowerShell**:
```powershell
ssh root@YOUR_DROPLET_IP
```

---

### 3️⃣ Deploy (Run on Droplet)

```bash
# Clone repository
git clone https://github.com/emiliskaupas/shop-main.git
cd shop-main

# Create environment file
cp .env.example .env
nano .env  # Edit: Set DB_PASSWORD and JWT_SECRET, then Ctrl+X, Y, Enter

# Deploy!
docker-compose up -d --build

# Wait 30 seconds, then run migrations
sleep 30
docker-compose exec backend dotnet ef database update
```

---

### 4️⃣ Open Your App

Go to: `http://YOUR_DROPLET_IP`

**Done! 🎉**

---

## Useful Commands

```bash
# View logs
docker-compose logs -f

# Restart
docker-compose restart

# Update app
git pull && docker-compose up -d --build

# Stop everything
docker-compose down

# Check status
docker-compose ps
```

---

## Environment Variables

Edit `.env` on your droplet:

```env
DB_PASSWORD=YourSecurePassword123!
JWT_SECRET=your-very-long-random-secret-key-at-least-64-chars
```

Generate secure JWT secret:
```bash
openssl rand -base64 64
```

---

## Troubleshooting

**Can't access application?**
```bash
# Open firewall
ufw allow 80
ufw allow 443
ufw allow 22
ufw enable
```

**Container not starting?**
```bash
# View specific logs
docker-compose logs backend
docker-compose logs frontend
docker-compose logs postgres
```

**Need to restart everything?**
```bash
docker-compose down
docker-compose up -d --build
```

---

## Cost

- **Droplet**: $12/month
- **With $200 credit**: FREE for ~16 months!

---

For detailed guide, see [DEPLOYMENT.md](DEPLOYMENT.md)
