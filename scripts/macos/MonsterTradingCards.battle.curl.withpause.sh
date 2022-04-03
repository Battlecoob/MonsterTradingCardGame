# --------------------------------------------------
# Monster Trading Cards Battle
# --------------------------------------------------
# title Monster Trading Cards Battle
echo "CURL Testing for Monster Trading Cards Battle"
echo .

# --------------------------------------------------
echo "17) battle"
start /b "kienboec battle" curl -i -X POST http://localhost:10001/battles --header "Authorization: Basic kienboec-mtcgToken"
start /b "altenhof battle" curl -i -X POST http://localhost:10001/battles --header "Authorization: Basic altenhof-mtcgToken"
ping localhost -n 10 >NUL 2>NUL

read -p "Press any key to resume ..."

# --------------------------------------------------
echo "18) Stats "
echo kienboec
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic kienboec-mtcgToken"
echo .
echo altenhof
curl -i -X GET http://localhost:10001/stats --header "Authorization: Basic altenhof-mtcgToken"
echo .
echo .

read -p "Press any key to resume ..."

# --------------------------------------------------
echo "19) scoreboard"
curl -i -X GET http://localhost:10001/score --header "Authorization: Basic kienboec-mtcgToken"
echo .
echo .

read -p "Press any key to resume ..."

# # --------------------------------------------------
# echo 20) trade
# echo check trading deals
# curl -i -X GET http://localhost:10001/tradings --header "Authorization: Basic kienboec-mtcgToken"
# echo .
# echo create trading deal
# curl -i -X POST http://localhost:10001/tradings --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}"
# echo .

# read -p "Press any key to resume ..."

# echo check trading deals
# curl -i -X GET http://localhost:10001/tradings --header "Authorization: Basic kienboec-mtcgToken"
# echo .
# curl -i -X GET http://localhost:10001/tradings --header "Authorization: Basic altenhof-mtcgToken"
# echo .

# read -p "Press any key to resume ..."

# echo delete trading deals
# curl -i -X DELETE http://localhost:10001/tradings/6cd85277-4590-49d4-b0cf-ba0a921faad0 --header "Authorization: Basic kienboec-mtcgToken"
# echo .
# echo .

# read -p "Press any key to resume ..."

# # --------------------------------------------------
# echo "21) check trading deals"
# curl -i -X GET http://localhost:10001/tradings  --header "Authorization: Basic kienboec-mtcgToken"
# echo .
# curl -i -X POST http://localhost:10001/tradings --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}"
# echo check trading deals
# curl -i -X GET http://localhost:10001/tradings  --header "Authorization: Basic kienboec-mtcgToken"
# echo .
# curl -i -X GET http://localhost:10001/tradings  --header "Authorization: Basic altenhof-mtcgToken"
# echo .

# read -p "Press any key to resume ..."

# echo "try to trade with yourself (should fail)"
# curl -i -X POST http://localhost:10001/tradings/6cd85277-4590-49d4-b0cf-ba0a921faad0 --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d "\"4ec8b269-0dfa-4f97-809a-2c63fe2a0025\""
# echo .

# read -p "Press any key to resume ..."

# echo try to trade 
# echo .
# curl -i -X POST http://localhost:10001/tradings/6cd85277-4590-49d4-b0cf-ba0a921faad0 --header "Content-Type: application/json" --header "Authorization: Basic altenhof-mtcgToken" -d "\"951e886a-0fbf-425d-8df5-af2ee4830d85\""
# echo .
# curl -i -X GET http://localhost:10001/tradings --header "Authorization: Basic kienboec-mtcgToken"
# echo .
# curl -i -X GET http://localhost:10001/tradings --header "Authorization: Basic altenhof-mtcgToken"
# echo .

# read -p "Press any key to resume ..."

# --------------------------------------------------
echo end...

# this is approx a sleep 
ping localhost -n 100 >NUL 2>NUL