var gulp = require('gulp');  //加载gulp
var uglify = require('gulp-uglify');  //加载js压缩
var concat = require("gulp-concat");

// 定义一个任务 compass
gulp.task('compass', async function () {
    gulp.src(['wwwroot/lib/sdk/core.js'])
        .pipe(uglify())
        .pipe(gulp.dest('wwwroot/lib/sdk/core_min'));
});

gulp.task('concat', async function () {
    gulp.src(['wwwroot/lib/sdk/vendor.js','wwwroot/lib/sdk/core_min/core.js'])  //要合并的文件
    .pipe(concat('oauthapp.1.0.0.js'))  // 合并匹配到的js文件并命名为 "all.js"
    .pipe(gulp.dest('wwwroot/lib/sdk'));
});