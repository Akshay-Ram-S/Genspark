const express = require('express');
const mongoose = require('mongoose');

const app = express();
const port = 3000;

const mongoUrl = 'mongodb://localhost:27017/db';

mongoose.connect(mongoUrl, {
  useNewUrlParser: true,
  useUnifiedTopology: true,
})
.then(() => console.log('Connected to MongoDB'))
.catch(err => console.error('MongoDB connection error:', err));

app.get('/', (req, res) => {
  res.send('Node.js API is running and connected to MongoDB');
});

app.listen(port, () => {
  console.log(`🚀 Server running on port ${port}`);
});
