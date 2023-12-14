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
        document.getElementById("name").placeholder = json.Name;
        document.getElementById("surname").placeholder = json.Surname;
        document.getElementById("location").placeholder = json.Location;
        if (JSON.parse(xhr.response).Image != null && JSON.parse(xhr.response).Image !== ""){
            document.getElementById("avatar-1").src = json.Image;
            document.querySelector(".photo").style.backgroundSize = `cover`;
            document.querySelector(".photo").style.backgroundRepeat = `no-repeat`;
            document.querySelector(".load").style.visibility = `hidden`;
            document.querySelector(".photo").style.border = `1px solid`
            document.querySelector(".photo").style.backgroundImage = `url('${json.Image}')`;
        }
    }
    xhr.send();
}
// при нажатии на кнопку
window.addEventListener("load", getData);

const form = document.getElementById("edit-form");
const data = {};
const save_btn = document.querySelector('.save');
save_btn.addEventListener('click', async (event) => {
    event.preventDefault();
    const formData = new FormData(form);
    const values = Object.fromEntries(formData.entries());
    Object.assign(data, values);
    console.log(data);
    await fetch('/edit', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(data)
    })
        .then(response => {
            if (response.ok) {
                window.location.href = 'http://localhost:8010/settings';
            } else {
                console.error('Произошла ошибка при сохранении информации о пользователе');
            }
        })
        .catch(error => {
            console.error('Произошла ошибка при отправке запроса', error);
        });
});

const photoInput = document.querySelector(".path");
photoInput.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.image = imageUrl;
        document.querySelector(".photo").style.backgroundSize = `cover`;
        document.querySelector(".photo").style.backgroundRepeat = `no-repeat`;
        document.querySelector(".load").style.visibility = `hidden`;
        document.querySelector(".photo").style.border = `1px solid`
        document.querySelector(".photo").style.backgroundImage = `url('${imageUrl}')`;

    }
    reader.readAsDataURL(file);
});

const delete_btn = document.querySelector('.delete-btn');
delete_btn.addEventListener('click', async (event) => {
    event.preventDefault();
    await fetch('/delete-account', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        }
    })
        .then(response => {
            if (response.ok) {
                // Переход на главную
                window.location.href = 'http://localhost:8010/home';
            } else {
                console.error('Произошла ошибка при сохранении информации о пользователе');
            }
        })
        .catch(error => {
            console.error('Произошла ошибка при отправке запроса', error);
        });
});
