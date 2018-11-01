var currentStreamState = "offline";
var info = {
	id: "",
	channel: ""
};

var hideTimer = null;

function addMessage (nick, mess) 
{
  var string = "<div class='container id='last_added'>" +
		"<span class='nickname'>" + nick + ": </span>" +
		"<span class='message'>" + mess + "</span>" +
		"</div>";

  send(string);
}

function addDonate (nick, mess, amount, value)
{
  var string = "<div class='container'>" +
		"<span class='nickname'>PandaChat: </span>" +
		"<span class='message'><span class='donate_nickname'>" + nick + "</span>" +
		" задонатил <span class='donate_amount'>" + amount + " (" + value + ")</span>" +
		": " + mess + "</span></div>";

  send(string);
}

function addFollower (nick)
{
	var string = "<div class='container'>" +
		"<span class='nickname'>PandaChat: </span>" +
		"<span class='message'><span class='follower_nickname'>" + nick + "</span> только что подписался!</span>" +
		"</div>";

  send(string);
}

function send(string)
{
  if(hideTimer !== null)
  {
    $(".chat_box").animate({
      opacity: "1"
    },
    {
      duration: 500
    });
  }

  $(".chat_box").append(string);

  // анимация появления
  var length = $(".container").length - 1;

  $(".container")[length].style.position = "relative";
  $(".container")[length].style.left = "200px";

  $(".container").animate({
    left: "0px"
  },
  {
    duration: 700
  });

  // скрытие чата после 5 минут неактива
  if(hideTimer !== null)
  {
    clearTimeout(hideTimer);
  }

  hideTimer = setTimeout(function () 
  { 
    $(".chat_box").animate({
      opacity: "0.4"
    },
    {
      duration: 500
    });
  }, 300000);

  setScroll();
}

function setScroll()
{
  $(".chat_box")[0].style.overflowY = "auto";

  setTimeout(function() {
    $(".chat_box")[0].scrollTop = $(".chat_box")[0].scrollHeight;

    $(".chat_box")[0].style.overflowY = "hidden";
  }, 10);
}

function onLoad()
{
  addMessage("PandaChat", "Добро пожаловать в PandaChat!");
  reloadSize();
}

function loadInfo(id, channel)
{
  info.id = id;
  info.channel = channel;
  checkStreamInfo();

  addMessage("PandaChat", "Панда успешно подключилась к " + channel);
}

function reloadSize()
{
  // для правильного отображения нужно менять
  // в браузере window.innerHeight
  // в приложение window.outerHeight
  // footer height - 4 = 22;
  $(".chat_box")[0].style.height = window.outerHeight - 22 + "px";
}

function checkStreamInfo()
{
  getStreamInfo();

  // рекурсивный setTimeout потому что setInterval прекращает работу спустя часа 2 работы чата
  var timer = setTimeout(function tick() {
    getStreamInfo();
    timer = setTimeout(tick, 13000);
  }, 13000);
}

function getStreamInfo() 
{
  $.ajax({
    type: 'GET',
    url: 'https://api.twitch.tv/kraken/streams/' + info.channel,
    headers: {
      'Client-ID': info.id
    },
    // dataType: json,
    success: function(data) {
      setStreamInfo(data);
    }
   });
}

function setStreamInfo(data)
{
  // console.log(data);
  if(data.stream === null)
  {
    if(currentStreamState !== "offline")
    {
      currentStreamState = "offline";

      $(".footer_text")[0].style.position = "relative";
      $(".footer_text")[0].style.top = "0px";

      $(".footer_text").animate({
        top: "50px"
      },
      {
        duration: 500
      });

      setTimeout(function() {
        $(".footer_text").text("offline");

        $(".footer_text").animate({
          top: "0px"
        },
        {
          duration: 500
        });
      }, 500);
    }
  }
  else
  {
    if(currentStreamState === "offline")
    {
      currentStreamState = "online";

      $(".footer_text")[0].style.position = "relative";
      $(".footer_text")[0].style.top = "0px";

      $(".footer_text").animate({
        top: "50px"
      },
      {
        duration: 500
      });

      setTimeout(function() {
        $(".footer_text").text(data.stream.viewers + " / " + data.stream.channel.followers);

        $(".footer_text").animate({
          top: "0px"
        },
        {
          duration: 500
        });
      }, 500);
    }
    else
    { 
      $(".footer_text").text(data.stream.viewers + " / " + data.stream.channel.followers);
    }
  }
}