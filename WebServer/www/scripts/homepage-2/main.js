function getData() {
    // URL на который будем отправлять GET запрос
    const requestURL = 'http://localhost:8010/get-info';
    const xhr = new XMLHttpRequest();
    xhr.open('GET', requestURL);
    xhr.onload = () => {
        if (xhr.status !== 200) {
            return;
        }
        let json = JSON.parse(xhr.response);
        document.getElementById("name").textContent = json.Name;
        if (JSON.parse(xhr.response).Image != null && JSON.parse(xhr.response).Image !== ""){
            document.getElementById("avatar").src = json.Image;
        }
        console.log(JSON.parse(xhr.response));
    }
    xhr.send();
}
// при нажатию на кнопку
window.addEventListener("load", getData);