#Stage 1: Build the React app
FROM node:18-alpine AS builder
WORKDIR /code-analyzer-service.ui
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

#Stage 2: Set up the Nginx and serve the build files
FROM nginx:stable-alpine
COPY --from=builder /code-analyzer-service.ui/build /usr/share/nginx/html
EXPOSE 3000
CMD ["nginx", "-g", "daemon off;"]