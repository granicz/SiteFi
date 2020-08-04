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
		},
	    uglify: { 
	        options: { 
	            compress: true 
	        }, 
	        applib: { 
	            src: [ 
	                'node_modules/@creativebulma/bulma-collapsible/dist/js/bulma-collapsible.min.js' 
	            ], 
	            dest: 'js/bulma-collapsible.js' 
	        } 
	    } 
	});

	grunt.loadNpmTasks('grunt-sass');
	grunt.loadNpmTasks('grunt-contrib-uglify');
	grunt.loadNpmTasks('grunt-contrib-cssmin');
	 
	grunt.registerTask('default', ['sass', 'cssmin', 'uglify']);
};