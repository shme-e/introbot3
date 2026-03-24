# intro bot 3

## dev setup

`dotnet user-secrets set "Discord:Token" "token"

open in dev container

download libdave from https://github.com/discord/libdave/releases and place into IntroBot3 folder

use vscode debugger (f5)

if everything is broken, f1 > reload window

## deploy setup

copy and fill .env.template

download libdave from https://github.com/discord/libdave/releases and place into IntroBot3 folder

`docker compose up`