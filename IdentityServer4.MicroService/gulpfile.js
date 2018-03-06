"use strict";
/**
 * 开发环境使用的打包脚本为：vendor、js、css
引用文件为：
vendor.min.js
vendor.min.css
app.js
app.css

 * 生产环境打包脚本：build
引用
app.min.js (合并vendor.js、app.js)
app.min.css (合并vendor.css、app.css)

发布到服务器只发布 build后的js和css
 */


var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    clean = require('gulp-clean'),
    sequence = require('gulp-sequence');

gulp.task("build", sequence('clean', ["vendor", "js", "css"],"combine"));

gulp.task("vendor", function () {

    var vendor_js = gulp.src([
        "./node_modules/jquery/dist/jquery.min.js",
        "./node_modules/bootstrap/dist/js/bootstrap.bundle.min.js",
        "./node_modules/swiper/dist/js/swiper.min.js",
        "./node_modules/jquery-validation/dist/jquery.validate.min.js",
        "./node_modules/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
        "./node_modules/angular/angular.min.js"])
        .pipe(concat('vendor.min.js'))
        .pipe(gulp.dest("./wwwroot/dist"));

    var vendor_css = gulp.src([
        './node_modules/bootstrap/dist/css/bootstrap.min.css',
        "./node_modules/swiper/dist/css/swiper.min.css",
    ])
        .pipe(concat('vendor.min.css'))
        .pipe(gulp.dest("./wwwroot/dist"));

    return merge(vendor_js, vendor_css);
});

gulp.task("js", function () {

    return gulp.src(["./wwwroot/js/app.js",])
        .pipe(concat('app.js'))
        .pipe(uglify())
        .pipe(gulp.dest("./wwwroot/dist"));
});

gulp.task("css", function ()
{
    return gulp.src('./wwwroot/css/app.css')
        .pipe(concat('app.css'))
        .pipe(cssmin())
        .pipe(gulp.dest("./wwwroot/dist"));
});

gulp.task("clean", function () {
    return gulp.src(['./dist']).pipe(clean({ force: true }));
});

gulp.task("combine", function () {

    var app_js = gulp.src([
        './wwwroot/dist/vendor.min.js',
        './wwwroot/dist/app.js'])
        .pipe(concat('app.min.js'))
        .pipe(gulp.dest("./wwwroot/dist"));

    var app_css = gulp.src([
        './wwwroot/dist/vendor.min.css',
        './wwwroot/dist/app.css'])
        .pipe(concat('app.min.css'))
        .pipe(gulp.dest("./wwwroot/dist"));

    return merge(app_js, app_css);
});

//gulp.task("watch", function () { });

//gulp.task("font", function () {
        //eot|otf|svg|ttf|woff|woff2
        //return gulp.src([]).pipe(gulp.dest("."));
//});