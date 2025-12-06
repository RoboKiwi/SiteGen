const path = require('path');

const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    optimization: {
        minimize: false,
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'PrismJS'
        })
    ]
};