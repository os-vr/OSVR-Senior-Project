#!/bin/bash
set -ev
echo "Testing Build Process"

doxygen Doxyfile

shopt -s extglob 
rm -rf !(html)
cd html
mv * ../
cd ..
rm -rf html

if [ "$TRAVIS_BRANCH" = "master" ]; then
	echo "Running API Docs, push to gh-pages"
  
	git config --global user.email "travis@travis-ci.org"
	git config --global user.name "Travis CI"

	git checkout -b gh-pages
	echo "Test" > doc.txt
	git add .
	git commit -m "Travis build: $TRAVIS_BUILD_NUMBER"

	ls

	git push -f https://${GITHUB_API_KEY}@github.com/os-vr/OSVR-Senior-Project.git
	  
fi

