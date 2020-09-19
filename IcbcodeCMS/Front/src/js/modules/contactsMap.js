export default function() {
  var $maps = document.querySelectorAll("[data-address]");
  if ($maps) {
    const appendYaMaps = () => {
      let $contacts = document.querySelector(".map");
      if ($contacts) {
        let contactsTop = $contacts.getBoundingClientRect().top;

        if (contactsTop <= 1000) {
          const key = "2315cab9-5a02-4018-a868-1baea0c875bf";
          const script = document.createElement("script");
          script.setAttribute("type", "text/javascript");
          script.src = `https://api-maps.yandex.ru/2.1/?apikey=${key}&lang=ru_RU&onload=initYaMaps`;

          if (!document.body.classList.contains("ya-maps-added")) {
            document.head.appendChild(script);
            document.body.classList.add("ya-maps-added");
          }
        }
      }
    };

    window.initYaMaps = function initYaMaps() {
      if ($maps.length > 0) {
        $maps.forEach(function(map) {
          var adr = map.dataset.address;

          function init() {
            var myGeocoder = ymaps.geocode(adr);

            myGeocoder.then(
              function(res) {
                var coord = res.geoObjects.get(0).geometry.getCoordinates();

                var myMap = new ymaps.Map(map, {
                  center: coord,
                  zoom: 16,
                  controls: [],
                });

                myMap.geoObjects.add(
                  new ymaps.Placemark(coord, {
                    balloonContentHeader: adr,
                  })
                );
              },
              function(err) {
                console.log("Error");
              }
            );
          }
          ymaps.ready(init);
        });
      }
    };

    window.addEventListener("scroll", function() {
      appendYaMaps();
    });
    appendYaMaps();
  }
}
