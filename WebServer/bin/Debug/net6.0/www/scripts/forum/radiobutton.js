function toggleElement() {
    let element1 = document.querySelector(".sightseeing");
    let element2 = document.querySelector(".other");
    let element3 = document.querySelector(".see-another")
    if (element1.style.display === "none") {
        element1.style.display = "block";
        element2.style.display = "block";
        element3.textContent = "❮ Скрыть"
    } else {
        element1.style.display = "none";
        element2.style.display = "none";
        element3.textContent = "Показать больше ❯"
    }
}