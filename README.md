# Spatial Filtering

[![](https://img.shields.io/github/last-commit/alexvarip/SpatialFiltering?style=plastic)](https://github.com/alexvarip/SpatialFiltering/)

A console application for filtering yuv image files with spatial filters. The image file size `[height x width]` must be entered manually after the application starts. <br><br>
```
Usage: executable [options[-f]] [filter] [options[-i]] [path-to-file]

options:
	-h | --help        Display help.
	-f | --filter      Select a filter.
	-i | --import      Import a file.	
		
filter: 
	Median, Average, Laplacian 

path-to-file: 
	The path to a .yuv file to import.
```
* `Notice: The application specifically targets only .yuv image files.`
