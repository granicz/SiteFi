<?php
/**
 * Your Twitter App Info
 */

$CONSUMER_KEY = "8lOdpwPJsJRo09gwBWJwdDAuK";
$CONSUMER_SECRET = "44W8EAIsxFu17JtSAu2nLipsS4tkuWD0PzIBtMSnKkXW2EC8G6";
$ACCESS_TOKEN = "209549641-NGDIgwNkP0oEqXuS2e9gGmo2LFIiPYbSlEjGaWJj";
$ACCESS_SECRET = "6WD9Vlu2OxSckZwoat6H8ohLQM2naxRZbk3k5kkgGbEH6";

if (file_exists("../../../../../../wp-load.php")) {
    include("../../../../../../wp-load.php");
    if (hc_is_setted("twitter-consumer-key")) $CONSUMER_KEY = hc_get_setting("twitter-consumer-key");
    if (hc_is_setted("twitter-consumer-secret")) $CONSUMER_KEY = hc_get_setting("twitter-consumer-secret");
    if (hc_is_setted("twitter-access-token")) $CONSUMER_KEY = hc_get_setting("twitter-access-token");
    if (hc_is_setted("twitter-access-secret")) $CONSUMER_KEY = hc_get_setting("twitter-access-secret");
}

// Consumer Key
define('CONSUMER_KEY', $CONSUMER_KEY);
define('CONSUMER_SECRET', $CONSUMER_SECRET);

// User Access Token
define('ACCESS_TOKEN', $ACCESS_TOKEN);
define('ACCESS_SECRET', $ACCESS_SECRET);

// Cache Settings
define('CACHE_ENABLED', false);
define('CACHE_LIFETIME', 3600); // in seconds
define('HASH_SALT', md5(dirname(__FILE__)));
