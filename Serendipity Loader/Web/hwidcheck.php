<?php
$hwid = $_GET["hwid"];
$db_username = "root";
$db_host = "localhost";
$db_password = "";
$db_name = "loader";
$mysql = new mysqli($db_host, $db_username, $db_password, $db_name);

if ($mysql->connect_errno) {
    printf("ERROR CONNECTING");
}
else {
    if (strlen($hwid) > 1){
        $sql = "SELECT * FROM loader WHERE hwid= '". \mysqli_real_escape_string($mysql,$hwid) ."'" ;
        $results = $mysql->query($sql);
        if ($results->num_rows > 0){
            while ($row = $results->fetch_assoc()){
                if ($row["hwid"] === $hwid){
                    echo "1";
                }
                else{
                    echo "0";
                }
            }
        }
    }   
}
?>