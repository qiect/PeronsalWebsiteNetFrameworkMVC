/**

 @Name: 回复模块

 */

layui.define('fly', function (exports) {

    var $ = layui.jquery;
    var layer = layui.layer;
    var util = layui.util;
    var laytpl = layui.laytpl;
    var form = layui.form;
    var fly = layui.fly;

    var dom = {
        jieda: $('#jieda')
        , content: $('#L_content')
        , jiedaCount: $('#jiedaCount')
        , love: $('#love')
    };



    //提交回答
    fly.form['/Article/Comments'] = function (data, required) {
        var tpl = '<li>\
      <div class="detail-about detail-about-reply">\
        <a class="fly-avatar" href="" target="_blank">\
          <img src="/res/images/avatar/0.jpg">\
        </a>\
        <div class="fly-detail-user">\
          <a target="_blank" class="fly-link">\
            <cite>小笨蛋</cite>\
          </a>\
        </div>\
        <div class="detail-hits">\
          <span>刚刚</span>\
        </div>\
      </div>\
      <div class="detail-body jieda-body photos">\
        '+ data.content + '\
      </div>\
    </li>';
        data.content = fly.content(data.content);
        laytpl(tpl).render($.extend(data, {
            user: layui.cache.user
        }), function (html) {
            required[0].value = '';
            required[1].value = '';
            dom.jieda.find('.fly-none').remove();
            dom.jieda.append(html);

            var count = dom.jiedaCount.text() | 0;
            dom.jiedaCount.html(++count);
        });
    };

    //文章操作
    dom.love.on('click', function () {
        fly.json('/Article/RateArticle/', { id: articleId }, function (res) {
            layer.msg(res.msg);
        });
    });

    //定位分页
    if (/\/page\//.test(location.href) && !location.hash) {
        var replyTop = $('#flyReply').offset().top - 80;
        $('html,body').scrollTop(replyTop);
    }

    exports('jie', null);
});