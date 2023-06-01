FROM node:18-alpine
WORKDIR /code-analyzer-service.ui
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build
EXPOSE 3000
CMD [ "npm", "run", "start" ]