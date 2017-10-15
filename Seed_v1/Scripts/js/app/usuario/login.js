function isMobile() {
    var userAgent = navigator.userAgent;

    return ((userAgent.match(/Android/i)) || (userAgent.match(/BlackBerry/i)) || (userAgent.match(/iPhone|iPad|iPod/i))
        || (userAgent.match(/Opera Mini/i)) || (userAgent.match(/IEMobile/i)));
}

function actionPassword(name) {
    var type = $("#txb" + name).attr("type");

    if (type == "password") {
        $("#txb" + name).attr("type", "text");
        $("#spn" + name).html("OCULTAR");
    }
    else {
        $("#txb" + name).attr("type", "password");
        $("#spn" + name).html("MOSTRAR");
    }
}

function loginUsuario(returnUrl) {
    var empresa = $("#txbEmpresa").val();
    var nombre = $("#txbNombre").val();
    var contrasenia = $("#txbContrasenia").val();
    var screenWidth = $(window).width();
    var screenHeight = $(window).height();

    var isMobil = false;

    if (isMobile()) {
        isMobil = true;
    }

    if (empresa.trim() != "") {
        $("#valEmpresa").css("display", "none");
        $("#txbEmpresa").removeClass("error");

        if (nombre.trim() != "") {
            $("#valNombre").css("display", "none");
            $("#txbNombre").removeClass("error");

            if (contrasenia.trim() != "") {
                $("#valContrasenia").css("display", "none");
                $("#txbContrasenia").removeClass("error");

                $("#alert-messages").css("display", "none");

                var model = {
                    Empresa: empresa.trim(),
                    Nombre: nombre.trim(),
                    Contrasenia: contrasenia.trim(),
                    OkRecordar: false,
                    ReturnUrl: returnUrl,
                    ScreenWidth: screenWidth,
                    ScreenHeight: screenHeight,
                    IsMobile: isMobil
                };

                var jsonModel = JSON.stringify(model);

                $("#process-messages").css("display", "block");
                $("#lblMensaje2").text("Validando Usuario..");

                $.ajax({
                    url: '/Usuario/Login2',
                    type: 'POST',
                    data: { jsonModel: jsonModel },
                    success: function (result) {
                        $("#process-messages").css("display", "none");

                        if (result.ok) {
                            window.location = result.newUrl;
                        }
                        else {
                            $("#alert-messages").css("display", "block");
                            $("#lblMensaje").text(result.message);
                        }
                    },
                    error: function (result) {

                    }
                });
            }
            else {
                $("#valContrasenia").css("display", "block");
                $("#valContrasenia").text("Contraseña es requerido.");
                $("#txbNombre").addClass("error");
            }
        }
        else {
            $("#valNombre").css("display", "block");
            $("#valNombre").text("Nombre es requerido.");
            $("#txbNombre").addClass("error");

            if (contrasenia.trim() == "") {
                $("#valContrasenia").css("display", "block");
                $("#valContrasenia").text("Contraseña es requerido.");
                $("#txbNombre").addClass("error");
            }
            else {
                $("#valContrasenia").css("display", "none");
                $("#txbContrasenia").removeClass("error");
            }
        }
    }
    else {
        $("#valEmpresa").css("display", "block");
        $("#valEmpresa").text("Organización es requerido.");
        $("#txbEmpresa").addClass("error");

        if (nombre.trim() == "") {
            $("#valNombre").css("display", "block");
            $("#valNombre").text("Nombre es requerido.");
            $("#txbNombre").addClass("error");
        }
        else {
            $("#valNombre").css("display", "none");
            $("#txbNombre").removeClass("error");
        }

        if (contrasenia.trim() == "") {
            $("#valContrasenia").css("display", "block");
            $("#valContrasenia").text("Contraseña es requerido.");
            $("#txbNombre").addClass("error");
        }
        else {
            $("#valContrasenia").css("display", "none");
            $("#txbContrasenia").removeClass("error");
        }
    }
}