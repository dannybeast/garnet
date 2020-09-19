import "lightgallery.js";
import "lg-video.js";
var youtubeThumbnail = require("youtube-thumbnail");

// let $videos = document.querySelectorAll(".js-video");
// for (let video of $videos) {
//     let url = video.getAttribute("href");
//     video.querySelector("img").setAttribute("src", youtubeThumbnail(url).high.url);
// }

export default function () {
    lightGallery(document.querySelector(".gallery"), {
        selector: ".gallery .js-gallery",
        subHtmlSelectorRelative: true
    });

    let videos = document.querySelectorAll(".js-video-wrapper");

    videos.forEach(video => {
        lightGallery(video, {
            selector: ".js-video",
            videoMaxWidth: "1200px",
            autoplayFirstVideo: true,
            youtubePlayerParams: {
                modestbranding: 0,
                showinfo: 0,
                controls: 1,
                rel: 0,
                autoplay: 1
            }
        });

    })
    


}