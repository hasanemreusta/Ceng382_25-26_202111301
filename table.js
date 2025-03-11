
// This part Creating by ChatGPT. OpenAI, 11 March 2025
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("class-form");
    const tableBody = document.querySelector("#class-table tbody");

    form.addEventListener("submit", function (event) {
        event.preventDefault();

        const className = document.getElementById("class-name").value.trim();
        const numPeople = document.getElementById("num-people").value.trim();
        const description = document.getElementById("description").value.trim();

        if (className === "" || numPeople === "" || description === "") {
            alert("All fields must be filled!");
            return;
        }

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${className}</td>
            <td>${numPeople}</td>
            <td>${description}</td>
        `;

        // // This part Creating by ChatGPT. OpenAI, 11 March 2025, prompt: "Row click event ekler misin?"
        row.addEventListener("click", function () {
            row.classList.toggle("selected-row");
        });

        // // This part Creating by ChatGPT. OpenAI, 11 March 2025, Mouseover & Mouseout Event: Temporary color change
        row.addEventListener("mouseover", function () {
            row.style.backgroundColor = "lightgreen";
        });

        row.addEventListener("mouseout", function () {
            row.style.backgroundColor = "";
        });

        // Double-click Event: Remove row
        row.addEventListener("dblclick", function () {
            row.remove();
        });

        tableBody.appendChild(row);

        // Reset form fields
        form.reset();
    });

    // Table Click Event: Log all class entries
    document.getElementById("class-table").addEventListener("click", function () {
        let rows = Array.from(document.querySelectorAll("#class-table tbody tr")).map(row => row.innerText);
        console.log("All class entries:", rows);
    });

    // Input Focus & Blur Events: Highlight input fields
    document.querySelectorAll("input, textarea").forEach(input => {
        input.addEventListener("focus", function () {
            input.style.border = "2px solid blue";
        });

        input.addEventListener("blur", function () {
            if (input.value.trim() === "") {
                input.style.border = "2px solid red";
            } else {
                input.style.border = "";
            }
        });
    });
});
