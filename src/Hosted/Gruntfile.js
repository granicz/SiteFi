module.exports = function(grunt) {
	const sass = require('node-sass');
	 
	grunt.initConfig({
	    cssmin: {
			build: {
			  src: 'css/all.css',
			  dest: 'css/all.min.css'
			}
		}
	});

	grunt.loadNpmTasks('grunt-sass');
	grunt.loadNpmTasks('grunt-contrib-uglify');
	grunt.loadNpmTasks('grunt-contrib-cssmin');
	 
	grunt.registerTask('default', ['cssmin']);
};