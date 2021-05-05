const path = require('path');

module.exports = {
    mode: "development",
    entry: {
        index: path.resolve(__dirname, "src/index.js"),
    },
    output: {
        path: path.resolve(__dirname, "../wwwroot/js"),
        filename: "[name].bundle.js",
        library: "MyModule"
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf|svg)$/,
                use: 'file-loader?name=../font/[name].[ext]'
            }
        ]
    }
};