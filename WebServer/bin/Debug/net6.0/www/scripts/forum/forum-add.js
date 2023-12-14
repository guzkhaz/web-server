const form = document.getElementById("post-form");
const data = {}
data.images = [];
var date = new Date();
data.date = `${date.getDate()}/${date.getMonth()+1}/${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}`
const save_btn = document.querySelector('.publish');
window.addEventListener("load", getData);

function deletePhotos(){
    for (var i = 0; i < 8; i++){
        data.images[i].remove;
       // document.getElementById(`photo-${i}`).style.backgroundSize = `cover`;
        //document.getElementById(`photo-${i}`).style.backgroundRepeat = `no-repeat`;
        document.getElementById(`load-${i}`).style.visibility = `visible`;
        document.getElementById(`photo-${i}`).style.border = `1px #707070 dashed`
        document.getElementById(`photo-${i}`).style.backgroundImage = `none`;
    }
}
save_btn.addEventListener('click', async (event) => {
    event.preventDefault();
    const formData = new FormData(form);
    const values = Object.fromEntries(formData.entries());
    if (values.title == "" || values.content == ""){
        alert("Заголовок и контент не могут быть пустыми");
    }
    else {
        Object.assign(data, values);
        data.tags = getTags();
        await fetch('/add-post', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (response.ok) {
                    window.location.href = 'http://localhost:8010/forum';
                } else {
                    console.error('Произошла ошибка при сохранении информации о пользователе');
                }
            })
            .catch(error => {
                console.error('Произошла ошибка при отправке запроса', error);
            });
    }
});
function getData() {
    const requestURL = 'http://localhost:8010/get-info';
    const xhr = new XMLHttpRequest();
    xhr.open('GET', requestURL);
    xhr.onload = () => {
        if (xhr.status !== 200) {
            return;
        }
        let json = JSON.parse(xhr.response);
        if (JSON.parse(xhr.response).Image != null && JSON.parse(xhr.response).Image !== ""){
            document.getElementById("avatar").src = json.Image;
        }
    }
    xhr.send();
}

function getTags(){
    let tags = document.getElementById("selectElement");
    let values = [];
    for (var i = 0; i < tags.options.length; i ++) {
        //если опция выбрана - добавим её в массив
        if (tags.options[i].selected)
            values.push(tags.options[i].value);
    }
    return values;
}

const photoInput = document.getElementById("path-1");
photoInput.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[0] = imageUrl;
        document.getElementById("photo-1").style.backgroundSize = `cover`;
        document.getElementById("photo-1").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-1").style.visibility = `hidden`;
        document.getElementById("photo-1").style.border = `1px solid`
        document.getElementById("photo-1").style.backgroundImage = `url('${imageUrl}')`;

    }
    reader.readAsDataURL(file);
});

const photoInput_1 = document.getElementById("path-2");
photoInput_1.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[1]= imageUrl;
        document.getElementById("photo-2").style.backgroundSize = `cover`;
        document.getElementById("photo-2").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-2").style.visibility = `hidden`;
        document.getElementById("photo-2").style.border = `1px solid`
        document.getElementById("photo-2").style.backgroundImage = `url('${imageUrl}')`;

    }
    reader.readAsDataURL(file);
});
const photoInput_2 = document.getElementById("path-3");
photoInput_2.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[2]= imageUrl;
        document.getElementById("photo-3").style.backgroundSize = `cover`;
        document.getElementById("photo-3").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-3").style.visibility = `hidden`;
        document.getElementById("photo-3").style.border = `1px solid`
        document.getElementById("photo-3").style.backgroundImage = `url('${imageUrl}')`;

    }
    reader.readAsDataURL(file);
});
const photoInput_3 = document.getElementById("path-4");
photoInput_3.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[3]= imageUrl;
        document.getElementById("photo-4").style.backgroundSize = `cover`;
        document.getElementById("photo-4").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-4").style.visibility = `hidden`;
        document.getElementById("photo-4").style.border = `1px solid`
        document.getElementById("photo-4").style.backgroundImage = `url('${imageUrl}')`;
    }
    reader.readAsDataURL(file);
});

const photoInput_4 = document.getElementById("path-5");
photoInput_4.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[4]= imageUrl;
        document.getElementById("photo-5").style.backgroundSize = `cover`;
        document.getElementById("photo-5").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-5").style.visibility = `hidden`;
        document.getElementById("photo-5").style.border = `1px solid`
        document.getElementById("photo-5").style.backgroundImage = `url('${imageUrl}')`;
    }
    reader.readAsDataURL(file);
});

const photoInput_5 = document.getElementById("path-6");
photoInput_5.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[5]= imageUrl;
        document.getElementById("photo-6").style.backgroundSize = `cover`;
        document.getElementById("photo-6").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-6").style.visibility = `hidden`;
        document.getElementById("photo-6").style.border = `1px solid`
        document.getElementById("photo-6").style.backgroundImage = `url('${imageUrl}')`;
    }
    reader.readAsDataURL(file);
});

const photoInput_6 = document.getElementById("path-7");
photoInput_6.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[6]= imageUrl;
        document.getElementById("photo-7").style.backgroundSize = `cover`;
        document.getElementById("photo-7").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-7").style.visibility = `hidden`;
        document.getElementById("photo-7").style.border = `1px solid`
        document.getElementById("photo-7").style.backgroundImage = `url('${imageUrl}')`;
    }
    reader.readAsDataURL(file);
});

const photoInput_7 = document.getElementById("path-8");
photoInput_7.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[7]= imageUrl;
        document.getElementById("photo-8").style.backgroundSize = `cover`;
        document.getElementById("photo-8").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-8").style.visibility = `hidden`;
        document.getElementById("photo-8").style.border = `1px solid`
        document.getElementById("photo-8").style.backgroundImage = `url('${imageUrl}')`;
    }
    reader.readAsDataURL(file);
});


const photoInput_8 = document.getElementById("path-9");
photoInput_8.addEventListener('change', async function(event){
    const file = event.target.files[0];
    let reader  = new FileReader();
    reader.onload = function(e)  {
        const imageUrl = e.target.result;
        data.images[8]= imageUrl;
        document.getElementById("photo-9").style.backgroundSize = `cover`;
        document.getElementById("photo-9").style.backgroundRepeat = `no-repeat`;
        document.getElementById("load-9").style.visibility = `hidden`;
        document.getElementById("photo-9").style.border = `1px solid`
        document.getElementById("photo-9").style.backgroundImage = `url('${imageUrl}')`;
    }
    reader.readAsDataURL(file);
});