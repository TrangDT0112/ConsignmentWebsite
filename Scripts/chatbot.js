const chatIcon = document.getElementById("chat-icon");
const chatBox = document.getElementById("chat-box");
const closeChat = document.getElementById("close-chat");

chatIcon.addEventListener("click", () => {
    chatBox.style.display = (chatBox.style.display === "none" || chatBox.style.display === "") ? "flex" : "none";
});

closeChat.addEventListener("click", () => {
    chatBox.style.display = "none";
});

document.getElementById("chat-form").addEventListener("submit", function (e) {
    e.preventDefault();
    const input = document.getElementById("userInput").value.trim();
    if (!input) return;

    const content = document.getElementById("chat-content");

    const userMsg = document.createElement("div");
    userMsg.className = "msg user";
    userMsg.innerText = input;
    content.appendChild(userMsg);

    const botMsg = document.createElement("div");
    botMsg.className = "msg bot";
    botMsg.innerText = "Chip is processing your request...";
    content.appendChild(botMsg);

    content.scrollTop = content.scrollHeight;

    fetch('/Home/AskTinyLlama', {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `userPrompt=${encodeURIComponent(input)}`
    })
        .then(response => response.json())
        .then(data => {
            botMsg.innerText = data.response;
            content.scrollTop = content.scrollHeight;
        })
        .catch(error => {
            botMsg.innerText = "❌ Error calling Chip.";
            console.error(error);
        });

    document.getElementById("userInput").value = "";
});