const form = document.getElementById("signup-form");
form.addEventListener("submit", async (event) => {
    event.preventDefault();
    const formData = new FormData(form);
    const user = Object.fromEntries(formData.entries());
    await fetch('/signup-1-registration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(user)
    })
        .then(response => {
        if (response.ok) {
            window.location.href = 'http://localhost:8010/signup-2';
        } 
        else {
            response.json().then((result) => {
                alert(result)
            });
        }})
        .catch(error => {
            console.error('Произошла ошибка при отправке запроса', error);
        });
});   

    