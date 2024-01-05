const path = require('path');

const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    optimization: {
        minimize: false,
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'PrismJS'
        })
    ]
};