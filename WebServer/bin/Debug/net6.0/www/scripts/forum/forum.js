const container = document.querySelector('.posts');
const template = document.querySelector('.post-shablon');

let posts = [];
let tagsBase = [];
let user = {};
function getValue(radio) {
    if (radio.value === "my"){
        const filtered = posts.filter(p => p.Creator === user.Email);
        document.querySelector(".posts").innerHTML = "";

        for (let i = 0; i < filtered.length; i++){
            showPosts(filtered[i]);
        }
    }
    else {
        document.querySelector(".posts").innerHTML = "";

        for (let i = 0; i < posts.length; i++){
            showPosts(posts[i]);
        }
    }
}

document.querySelector('.tags').addEventListener('change', function() {
    const values = Array.from(
        this.querySelectorAll('.tagCheckbox:checked'),
        n => n.name
    );
    
    if(values.length === 0){
        document.querySelector(".posts").innerHTML = "";
        
        for (let i = 0; i < posts.length; i++){
            showPosts(posts[i]);
        }
    }
    else {
        const filtered = values.length
            ? posts.filter(n => values.every(m => n.Tags.includes(m)))
            : [];

        document.querySelector(".posts").innerHTML = "";

        for (let i = 0; i < filtered.length; i++){
            showPosts(filtered[i]);
        }
    }
});

function redirectToAddPage(){
    window.location.href = "http://localhost:8010/forum-add";
}

function showPosts(data){
    const clone = template.content.cloneNode(true);
    
    const title = clone.querySelector('.post-title');
    clone.querySelector('.post-content').setAttribute("id", `${data.Id}`);
    title.setAttribute("id", `${data.Id}`);
    const content = clone.querySelector('.post-text');
    const creator = clone.querySelector('.post-name');
    const date = clone.querySelector('.post-time');
    const tags = clone.querySelector('.post-tags');
    title.innerHTML = data.Title;
    content.innerHTML = data.Content.slice(0, 150) + "...";
    creator.innerHTML = data.CreatorName;
    date.innerHTML = data.Date;
    if (data.CreatorImage != null && data.CreatorImage !== ""){
        clone.querySelector(".avatar").src = data.CreatorImage;
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

    container.appendChild(clone);
}

function getPosts() {
    const requestURL = 'http://localhost:8010/get-posts';
    const xhr = new XMLHttpRequest();
    xhr.open('GET', requestURL);
    xhr.onload = () => {
        if (xhr.status !== 200) {
        }
        let json = JSON.parse(xhr.response);
        posts = json;
        for (let i = 0; i < json.length; i++) {
            showPosts(json[i]);
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
        }
    }
    xhr.send();
}
function getData() {
    // URL на который будем отправлять GET запрос
    getTags();
    getUser();
    getPosts();
}
// при нажатию на кнопку
window.addEventListener("load", getData);

function redirectToPost(event)
{
    let url = new URL('http://localhost:8010/forum-post');
    let postId = event.target.id;
    url.searchParams.set('id', postId);
    window.location.href = url;
}
