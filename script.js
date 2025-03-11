// This  part Creating by ChatGPT. OpenAI, 4 March 2025

function updateClock() {
    const now = new Date();
    let hours = now.getHours().toString().padStart(2, '0');
    let minutes = now.getMinutes().toString().padStart(2, '0');
    let seconds = now.getSeconds().toString().padStart(2, '0');
  
    document.getElementById('clock').innerText = `${hours}:${minutes}:${seconds}`;
  }
  
  
  setInterval(updateClock, 1000);
  updateClock(); 
  
  
  let loginData = [];
  
  document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.querySelector("form");

    loginForm.addEventListener("submit", function (event) {
        event.preventDefault();

        const username = document.querySelector('input[type="text"]').value;
        const password = document.querySelector('input[type="password"]').value;

        if (username === "admin" && password === "admin") {
            window.location.href = "table.html"; // Redirect to table.html
        } else {
            alert("Invalid Credentials. Try again!");
        }
    });
  
    // This  part Creating by ChatGPT. OpenAI, 4 March 2025
    document.addEventListener("keydown", function (event) {
      if (event.key.toLowerCase() === "h") { 
        const form = document.querySelector("form");
  

        if (form.style.display === "none") {
          form.style.display = "block";
        } else {
          form.style.display = "none";
        }
      }
    });
  });
  