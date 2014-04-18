/// <reference path="http://cdn.bootcss.com/jquery/1.10.2/jquery.min.js"/>
/// <reference path="http://cdn.bootcss.com/twitter-bootstrap/3.0.3/js/bootstrap.min.js"/>
/// <reference path="http://malsup.github.io/jquery.form.js"/>
$(function () {
    $("#slider").slider({ max: 100 });
    $("#myModalLabel").text("0");
    $(".scoring_content_score").on("click", function () {
        $(this).select();
    })
    $("#slider").on("slide", function (event, ui) {
        $("#myModalLabel").text($(this).slider("value") + 1);
    });

    //...按钮点击
    $(".scoring_content_score_click").on("click", function () {
        $("#myModalLabel").attr("data-target", $(this).parent().parent().index());
        var score = $(this).parent().find(".scoring_content_score").val();
        $("#slider").slider("value", score); //slider进度条
        $("#myModalLabel").text(score) //弹框标题
        $("#myModal").modal();
    });

    //只能输入0~100数字
    $(".scoring_content_score").blur(function () {
        if ($(this).val() == "") {
            $(this).val("0")
        } else {
            var score = parseInt($(this).val());
            if (score > 100) {
                $(this).val("100");
            }
        }
    });

    $("#score_submit").on("click", function () {
        var IsValid = false;
        $("#scoring_content .datarow").each(function (i, item) {
            var BeRatedEmployeeId = $(item).find(".hdemployeeid").val();
            var Scores = $(item).find(".scoring_content_score").val();
            var Suggest = $(item).find(".suggest").val();

            var intScores = parseInt(Scores);

            if (intScores > 0 && intScores <= 100) {
                IsValid = true;
                $(item).find(".postdata").val(BeRatedEmployeeId + "," + Scores + "," + Suggest);
            } else {
                IsValid = false;
                $(item).find(".scoring_content_score").css("border-color", "red");
                return false;
            }
        });
        if (IsValid) {
            var postdatas = "";
            $("#scoring_content .postdata").each(function (i, item) {
                postdatas += $(item).val() + ";";
            });

            $.ajax({
                type: "post",
                url: "/Home/Submit",
                data: "postdata=" + postdatas,
                success: function (data) {
                    if (data.IsSuccess) {
                        window.location.href = "/";
                    }
                    return false;
                },
                error: function (msg) {
                    return false;
                }
            });
        }

    })



    $(".s_submit").on("click", function () {
        var intscore = parseInt($("#myModalLabel").text());
        var _index = $("#myModalLabel").attr("data-target");
        $(".scoring_content_score").eq(_index).val(intscore);
        $('#myModal').modal('hide');
    });

})