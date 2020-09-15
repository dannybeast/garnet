var hideTime = 2000;
const noticeElement = document.createElement('div');
noticeElement.classList.add('notice');
document.body.appendChild(noticeElement);

class Notice {
    close() {
        noticeElement.classList = "";
        noticeElement.classList.add('notice');
        noticeElement.innerHTML = "";
    }
    openSuccess(text, delay) {
        noticeElement.innerHTML = text;
        noticeElement.classList.add('notice--green', 'notice--open');
        if(delay){
            hideTime = delay
        }
        setTimeout(this.close, hideTime);
    }
    openError(text, delay) {
        noticeElement.innerHTML = text;
        noticeElement.classList.add('notice--red', 'notice--open');
        if(delay){
            hideTime = delay
        }
        setTimeout(this.close, hideTime);
    }
}

export default new Notice();

window.Notice = new Notice();