#!/bin/bash

echo -e "Input: '$1'\n"

SNAKE=$(echo $1 | sed "s/\([A-Z]\)/_\L\1/g;s/^_//;s/-/_/g;s/_\+/_/g")
KEBAB=$(echo $SNAKE | sed -r "s/_/-/g")
PASCAL=$(echo $SNAKE | sed -r "s/(^|_)([a-z])/\U\2/g")
CAMEL=$(echo $PASCAL | sed -r "s/^([A-Z])/\L\1/g")

echo "Renaming all 'base_api' -> '$SNAKE'"
echo "Renaming all 'bonuscalc-api' -> '$KEBAB'"
echo "Renaming all 'BonusCalcApi' -> '$PASCAL'"
echo "Renaming all 'BonusCalcApi' -> '$CAMEL'"

echo -e "\nRenaming in $PWD.\n"
read -p "Does this sound OK? " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
  echo "Cancelled."
  exit 1
fi

# folder
find . -type d -not -path '*/\.git/*' | sed -e "p;s/base_api/$SNAKE/" | xargs -n2 mv
find . -type d -not -path '*/\.git/*' | sed -e "p;s/bonuscalc-api/$KEBAB/" | xargs -n2 mv
find . -type d -not -path '*/\.git/*' | sed -e "p;s/BonusCalcApi/$PASCAL/" | xargs -n2 mv
find . -type d -not -path '*/\.git/*' | sed -e "p;s/BonusCalcApi/$CAMEL/" | xargs -n2 mv
# file names
find . -type f -not -path '*/\.git/*' | sed -e "p;s/base_api/$SNAKE/" | xargs -n2 mv
find . -type f -not -path '*/\.git/*' | sed -e "p;s/bonuscalc-api/$KEBAB/" | xargs -n2 mv
find . -type f -not -path '*/\.git/*' | sed -e "p;s/BonusCalcApi/$PASCAL/" | xargs -n2 mv
find . -type f -not -path '*/\.git/*' | sed -e "p;s/BonusCalcApi/$CAMEL/" | xargs -n2 mv
# strings
find . -type f -not -path '*/\.git/*' -exec sed -i "s/base_api/$SNAKE/g" {} \;
find . -type f -not -path '*/\.git/*' -exec sed -i "s/bonuscalc-api/$KEBAB/g" {} \;
find . -type f -not -path '*/\.git/*' -exec sed -i "s/BonusCalcApi/$PASCAL/g" {} \;
find . -type f -not -path '*/\.git/*' -exec sed -i "s/BonusCalcApi/$CAMEL/g" {} \;
