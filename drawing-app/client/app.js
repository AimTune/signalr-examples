const canvas = document.getElementById("canvas");
const ctx = canvas.getContext("2d");
const eraser = document.getElementById("eraser");
const colorPicker = document.getElementById("colorPicker");
const eraseButton = document.getElementById("eraseButton");

let color = "#df4b26";
let paint = false;

ctx.lineWidth = 3;
ctx.strokeStyle = color;

function draw(mouseX, mouseY, colorFromEvent, lineWidth, event = null) {
  if (!event) {
    if (eraser.checked) {
      ctx.lineWidth = 10;
      ctx.strokeStyle = "#FFF";
    } else {
      ctx.lineWidth = 3;
      ctx.strokeStyle = color;
    }
  } else {
    ctx.lineWidth = lineWidth || ctx.lineWidth;
    ctx.strokeStyle = colorFromEvent || color;
  }
  ctx.lineJoin = "round";
  ctx.beginPath();

  ctx.moveTo(mouseX - 1, mouseY - 1);

  ctx.lineTo(mouseX, mouseY);
  ctx.closePath();
  ctx.stroke();
}

canvas.addEventListener("mousemove", (e) => {
  if (paint) {
    const mouseX = e.pageX - canvas.offsetLeft;
    const mouseY = e.pageY - canvas.offsetTop;
    draw(mouseX, mouseY, color);
    connection.invoke(
      "DrawColor",
      ctx.strokeStyle,
      mouseX,
      mouseY,
      ctx.lineWidth
    );
  }
});

canvas.addEventListener("mousedown", (e) => {
  paint = true;
});

canvas.addEventListener("mouseup", (e) => {
  paint = false;
});

canvas.addEventListener("mouseleave", (e) => {
  paint = false;
});

colorPicker.addEventListener("input", (e) => {
  color = e.target.value;
});

eraseButton.addEventListener("click", () => {
  clearScreen();
  fetch("https://localhost:7071/erase");
});

function clearScreen() {
  ctx.clearRect(0, 0, 500, 500);
}

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7071/drawinghub", {
    skipNegotiation: true,
    transport: 1,
  })
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function start() {
  try {
    await connection.start();
    console.log("SignalR Connected.");
  } catch (err) {
    console.log(err);
    setTimeout(start, 5000);
  }
}

connection.on("DrawColor", (color, x, y, lineWidth) => {
  draw(x, y, color, lineWidth, true);
});

connection.on("EraseAllScreen", () => {
  clearScreen();
});

connection.onclose(async () => {
  await start();
});

start();
