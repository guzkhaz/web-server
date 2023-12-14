const form = document.getElementById("signup-form");
const data = {};
form.addEventListener("submit", async (event) => {
    event.preventDefault();
    const formData = new FormData(form);
    const values = Object.fromEntries(formData.entries());
    Object.assign(data, values);
    
    await fetch('/signup-1-supplement', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(data)
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
