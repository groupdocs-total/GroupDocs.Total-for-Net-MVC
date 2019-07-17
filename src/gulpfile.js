var gulp = require('gulp')
gulp.task('copy', function () {
    return gulp
        .src('./node_modules/@groupdocs.examples.jquery/**')
        .pipe(gulp.dest('./Resources/'))   
})

var exec = require('child_process').exec;

gulp.task('build', function () {
    return exec('npm run build-client', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
})