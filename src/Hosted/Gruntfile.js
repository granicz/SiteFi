module.exports = function(grunt) {
	const sass = require('node-sass');
	 
	grunt.initConfig({
	    sass: {
	        options: {
	            implementation: sass
	        },
	        dist: {
	            files: {
	                'css/all.css': 'scss/all.scss'
	            }
	        }
	    },
	    cssmin: {
			build: {
			  src: 'css/all.css',
			  dest: 'css/all.min.css'
			}
		}
	});

	grunt.loadNpmTasks('grunt-sass');
	grunt.loadNpmTasks('grunt-contrib-cssmin');
	 
	grunt.registerTask('default', ['sass', 'cssmin']);
};