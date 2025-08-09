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

# Accept build arguments
ARG REACT_APP_API_URL
ARG REACT_APP_AUTH0_DOMAIN
ARG REACT_APP_AUTH0_CLIENT_ID
ARG REACT_APP_AUTH0_REDIRECT_URI

# Set environment variables for the build
ENV REACT_APP_API_URL=$REACT_APP_API_URL
ENV REACT_APP_AUTH0_DOMAIN=$REACT_APP_AUTH0_DOMAIN
ENV REACT_APP_AUTH0_CLIENT_ID=$REACT_APP_AUTH0_CLIENT_ID
ENV REACT_APP_AUTH0_REDIRECT_URI=$REACT_APP_AUTH0_REDIRECT_URI

# Copy source code
COPY . .

# Build the app (environment variables are embedded here)
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