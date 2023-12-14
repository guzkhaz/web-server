let tagsBase = [];
function showPosts(data){
    const title = document.querySelector('.post-title');
    const content = document.querySelector('.post-text');
    const creator = document.querySelector('.post-name');
    const date = document.querySelector('.post-time');
    const tags = document.querySelector('.post-tags');
    const images = document.querySelector('.post-images');
    title.innerHTML = data.Title;
    content.innerHTML = data.Content;
    creator.innerHTML = data.CreatorName;
    date.innerHTML = data.Date;
    if (data.CreatorImage != null && data.CreatorImage !== ""){
        document.querySelector(".post-avatar").src = data.CreatorImage;
    }
    for(let i = 0; i < data.Tags.length; i++){
        let element = document.createElement('div');
        element.className = "tag";
        let tagColor = tagsBase.find(tag => tag.Value === data.Tags[i]).Color;
        let tagName = tagsBase.find(tag => tag.Value === data.Tags[i]).Name;
        element.innerHTML = tagName;
        element.style.backgroundColor = tagColor;
        tags.appendChild(element);
    }
    if (data.Images != null && data.Images.length !== ""){
        for (let i = 0; i < data.Images.length; i++){
            let picture = document.createElement('img');
            picture.className = "image";
            picture.src = data.Images[i];
            //picture.style.backgroundImage =`url('${data.Images[i]}')`;
            images.appendChild(picture);
        }
    }
}
const container = document.querySelector('.comments-container');
const template = document.querySelector('.comment-shablon');
function showComments(data){
    const clone = template.content.cloneNode(true);
    const creatorName = clone.querySelector('.comment-name');
    const creatorImage = clone.querySelector('.comment-avatar');
    const content = clone.querySelector('.comment-text');
    const date = clone.querySelector('.comment-time');
    content.innerHTML = data.Content;
    creatorName.innerHTML = data.CreatorName;
    date.innerHTML = data.Date;
    creatorName.innerHTML = data.CreatorName;
    if (data.CreatorImage != null && data.CreatorImage !== ""){
        creatorImage.src = data.CreatorImage;
    }
    container.appendChild(clone);
}
function getPost(){

    let url = new URL('http://localhost:8010/get-post-by-id');
    let params = (new URL(document.location)).searchParams;
    url.searchParams.set('id', params.get("id"));
    
    const xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.onload = () => {
        if (xhr.status !== 200) {
            return;
        }
        let json = JSON.parse(xhr.response);
        showPosts(json);
    }
    xhr.send();
}
function getUser(){
    const requestURL = 'http://localhost:8010/get-info';
    const xhr = new XMLHttpRequest();
    xhr.open('GET', requestURL);
    xhr.onload = () => {
        if (xhr.status !== 200) {
            return;
        }
        let json = JSON.parse(xhr.response);
        user = json;
        if (user.Image != null && user !== ""){
            document.getElementById("avatar").src = json.Image;
            document.getElementById("avatar-comment").src = json.Image;
        }
    }
    xhr.send();
}
function getComments(){
    let url = new URL('http://localhost:8010/get-comments');
    let params = (new URL(document.location)).searchParams;
    url.searchParams.set('id', params.get("id"));

    const xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.onload = () => {
        if (xhr.status !== 200) {
            return;
        }
        let json = JSON.parse(xhr.response);
        for (let i = 0; i < json.length; i++) {
            showComments(json[i]);
        }
    }
    xhr.send();
}
function getTags(){
    const requestURL = 'http://localhost:8010/get-tags';
    const xhr = new XMLHttpRequest();
    xhr.open('GET', requestURL);
    xhr.onload = () => {
        if (xhr.status !== 200) {
        }
        let json = JSON.parse(xhr.response);
        tagsBase = json;
    }
    xhr.send();
}

function getData() {
    getTags();
    getUser();
    getPost();
    getComments();
}
window.addEventListener("load", getData);

const form = document.querySelector(".comment-form")
form.addEventListener("submit", async (event) => {
    event.preventDefault();
    const formData = new FormData(form);
    const values = Object.fromEntries(formData.entries());
    let date = new Date();
    values.date = `${date.getDate()}/${date.getMonth()+1}/${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}`
    if (values.comment === "") {
        alert("Комментарий не может быть пустым");
    }
    else {
        await fetch('/add-comment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8',
                'Post-Id': (new URL(document.location)).searchParams.get('id')
            },
            body: JSON.stringify(values)
        })
            .then(response => {
                if (response.ok) {
                    location.reload();
                } else {
                    response.json().then((result) => {
                        alert(result)
                    });
                }
            })
            .catch(error => {
                console.error('Произошла ошибка при отправке запроса', error);
            });
    }
}); 


    