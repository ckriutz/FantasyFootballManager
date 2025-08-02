# Use a more recent Node.js version with better performance
FROM node:18-alpine AS base

# Install dependencies only when needed
FROM base AS deps
WORKDIR /app

# Copy package files
COPY package.json package-lock.json* ./

# Install dependencies with npm ci for faster, reliable builds
RUN npm ci --only=production && npm cache clean --force

# Development dependencies for building
FROM base AS builder
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm ci

# Copy source code
COPY . .

# Build the app
RUN npm run build

# Production image, copy all the files and run the app
FROM base AS runner
WORKDIR /app

# Install serve to run the built app
RUN npm install -g serve

# Copy built application
COPY --from=builder /app/build ./build

EXPOSE 3000

# Use serve to serve the built static files
CMD ["serve", "-s", "build", "-l", "3000"]