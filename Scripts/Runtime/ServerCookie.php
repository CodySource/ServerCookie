<?php
	if (!isset($_GET['Cookie'])) o(null,'Missing cookie identifier.');
	if (!isset($_COOKIE[$_GET['Cookie']])) o(null, 'Cookie not found.');
	o($_COOKIE[$_GET['Cookie']],null);
	function o($v,$e){die(json_encode((object)array('value'=>$v,'error'=>$e)));}
?>