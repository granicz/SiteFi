<?php
header('Access-Control-Allow-Origin: *');

$isWP = false;
if (file_exists("../../../../../wp-load.php")) {
    include("../../../../../wp-load.php");
    $isWP = true;
}

$emailTo       = '';
$sender_email = 'noreply@example.com';
$subject = 'You received a new message';

$errors = array();
$data = array();
$body = '';
$email = '';
$name = '';
$domain = '';
$grecaptcha_secret_key = '[YOUR-KEY-HERE]';

if (isset($_POST['email'])) $domain = $_POST['domain'];
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $arr = $_POST['values'];
    $sender_email = 'contacts@' . $domain;
    $email = 'no-replay@' . $domain;
    $error = "Error. Messagge not sent.";
    $grecaptcha_valid = true;

    if (isset($_POST['grecaptcha']) && $_POST['grecaptcha'] != '') {
        $ip = $_SERVER['REMOTE_ADDR'];
        $url = 'https://www.google.com/recaptcha/api/siteverify';
        $data = array('secret' => $grecaptcha_secret_key, 'response' => $_POST['grecaptcha']);
        $options = array(
            'http' => array(
            'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
            'method'  => 'POST',
            'content' => http_build_query($data)
          )
        );
        $context  = stream_context_create($options);
        $response = file_get_contents($url, false, $context);
        $response_keys = json_decode($response, true);

        if (!$response_keys['success']) {
            $data['success'] = false;
            $data['message'] = 'grecaptcha-error';
            $grecaptcha_valid = false;
        }
    }

    if ($grecaptcha_valid) {
        if (isset($_POST['email']) && strlen($_POST['email']) > 0)  $emailTo = $_POST['email'];
        if (isset($_POST['subject_email']) && strlen($_POST['subject_email']) > 0) $subject = $_POST['subject_email'];
        else $subject = '[' . $domain . '] New message';

        foreach ($arr as $key => $value ) {
            $val =  stripslashes(trim($value[0]));
            if (!empty($val)) {
                $body .= ucfirst($key) . ': ' . $val . PHP_EOL . PHP_EOL;
                if ($key == "email"||$key == "Email"||$key == "E-mail"||$key == "e-mail"||strpos($key, "mail") > -1) $email = $val;
                if ($key == "name"||$key == "nome"||$key == "Nome") $name = $val;
            }
        }
        $body .= "-------------------------------------------------------------------------------------------" . PHP_EOL . PHP_EOL;
        $body .= "New message from " . $domain;

        if ($name == '') $name = $subject;
        if (!empty($errors)) {
            $data['success'] = false;
            $data['errors']  = $errors;
        } else {
            $headers  = "From: " . $email . "\r\n";
            $headers .= "Reply-To: " . $email . "\r\n";
            $result;
            $config;
            if ((isset($_POST['engine']) && $_POST['engine'] == "smtp") || ($isWP && hc_get_setting("smtp-host") != "")) {
                require 'phpmailer/PHPMailerAutoload.php';
                if ($isWP) {
                    $config = array("host" => hc_get_setting("smtp-host"),"username" => hc_get_setting("smtp-username"),"password" => hc_get_setting("smtp-psw"),"port" => hc_get_setting("smtp-port"),"email_from" => hc_get_setting("smtp-email"));
                } else {
                    require 'phpmailer/config.php';
                    $config = $smtp_config;
                }
                $mail = new PHPMailer;
                $message = nl2br($body);
                $mail->isSMTP();
                $mail->Host = $config["host"];
                $mail->SMTPAuth = true;
                $mail->Username = $config["username"];
                $mail->Password = $config["password"];
                $mail->SMTPSecure = 'ssl';
                $mail->Port = $config["port"];
                $mail->setFrom($config["email_from"]);
                if (strpos($emailTo,",") > 0) {
                    $arr = explode(",",$emailTo);
                    for ($i = 0; $i < count($arr); $i++) {
                        $mail->addAddress($arr[$i]);
                    }
                } else {
                    $mail->addAddress($emailTo);
                }
                $mail->isHTML(true);
                $mail->Subject = $subject;
                $mail->Body    = $message;
                $mail->AltBody = $message;
                $result = $mail->send();
                if (!$result) $error = $mail->ErrorInfo;
            } else {
                if ($isWP) {
                    try {
                        $result = wp_mail($emailTo, $subject, $body, $headers);
                    }
                    catch (Exception $exception) {
                        $result = mail($emailTo, $subject, $body, $headers);
                    }
                } else {
                    ini_set("sendmail_from", $email);
                    $result = mail($emailTo, $subject, $body, $headers);
                }
            }
            if ($result) {
                $data['success'] = true;
                $data['message'] = 'Congratulations. Your message has been sent successfully.';
            } else {
                $data['success'] = false;
                $data['message'] = $error;
            }
        }
    }
    echo json_encode($data);
}
