/*
 
 The MIT License (MIT)
 
 Copyright (c) 2014 Martin Reinhardt
 Copyright (c) 2015 Daniel Jarvis
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
 
 Certificate Plugin for Cordova Compatibile with Crosswalk v10.x
 
 */
package com.danjarvis.cordova.plugins;

import org.apache.cordova.CallbackContext;
import org.apache.cordova.CordovaActivity;
import org.apache.cordova.CordovaPlugin;
import org.apache.cordova.CordovaWebViewImpl;
import org.apache.cordova.CordovaWebViewEngine;
import org.json.JSONArray;
import org.json.JSONException;

import org.crosswalk.engine.XWalkWebViewEngine;
import org.crosswalk.engine.XWalkCordovaView;

import android.util.Log;

/**
 * 
 * Certificate Plugin for Cordova
 * 
 * authors: Martin Reinhardt on 23.06.14
 *          Daniel Jarvis updated on 2015-05-08
 * 
 * Copyright Martin Reinhardt 2014
 * Copyright Daniel Jarvis 2015
 *
 * All rights reserved.
 * 
 */
public class CertificatesPlugin extends CordovaPlugin {

    /**
     * Logging Tag
     */
    private static final String LOG_TAG = "Certificates";

    /**
     * Untrusted Variable
     */
    private boolean allowUntrusted = false;

    /**
     * Executes the request.
     * 
     * This method is called from the WebView thread. To do a non-trivial amount
     * of work, use: cordova.getThreadPool().execute(runnable);
     * 
     * To run on the UI thread, use:
     * cordova.getActivity().runOnUiThread(runnable);
     * 
     * @param action
     *            The action to execute. (Currently "setUntrusted only")
     * @param args
     *            The exec() arguments.
     * @param callbackContext
     *            The callback context used when calling back into JavaScript.
     * @return Whether the action was valid.
     * 
     * 
     */
    @Override
    public boolean execute(String action, JSONArray args,
            CallbackContext callbackContext) throws JSONException {
        
        if (action.equals("setUntrusted")) {
                allowUntrusted = args.getBoolean(0);
                cordova.getActivity().runOnUiThread(new Runnable() {
                        public void run() {
                            CordovaActivity ca = (CordovaActivity) cordova.getActivity();
                            XWalkCordovaView view = (XWalkCordovaView)webView.getView();
                            CertificatesCordovaWebViewClient cWebClient =
                                new CertificatesCordovaWebViewClient((XWalkWebViewEngine)webView.getEngine());

                            cWebClient.setAllowUntrusted(allowUntrusted);
                            webView.clearCache();
                            view.setResourceClient(cWebClient);
                        }
                });
                callbackContext.success();
                return true;
        }
        callbackContext.error("Invalid Command");
        return false;
    }
}
