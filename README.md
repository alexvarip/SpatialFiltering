# SpatialFiltering
A console application for filtering yuv image files with spatial filters. The image file size [height x width] must be entered manually after the application starts. <br><br>

Usage: executable [options[-f]] [filter] [options[-i]] [path-to-file]

Options:<br>
&emsp;-h | --help  &emsp;&emsp;&emsp; Display help.<br>
&emsp;-f&nbsp; | --filter  &emsp;&emsp;&emsp; Select a filter.<br>
&emsp;-i&nbsp; | --import  &emsp;&emsp; Import a file.<br>
filter: <br>
&emsp;Median, Average, Laplacian <br>
path-to-file: <br>
&emsp;The path to a .yuv file to import.<br>

<br> Notice: The application specifically targets only .yuv image files.
