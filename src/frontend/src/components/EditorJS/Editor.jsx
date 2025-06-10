import React, { useEffect } from 'react';
import EditorJS from '@editorjs/editorjs';
import { EDITOR_JS_CONFIG } from './EditorConfig';

const Editor = ({ onChange, initialData, readOnly = false, editorRef }) => {
    useEffect(() => {
        if (!editorRef.current) {
            editorRef.current = new EditorJS({
                ...EDITOR_JS_CONFIG,
                holder: 'editorjs',
                readOnly,
                data: initialData || EDITOR_JS_CONFIG.data,
                onChange: async () => {
                    const data = await editorRef.current.save();
                    onChange?.(data);
                }
            });
        }

        return () => {
            if (editorRef.current && editorRef.current.destroy) {
                editorRef.current.destroy();
            }
        };
    }, [onChange, initialData, readOnly, editorRef]);

    return (
        <div 
            id="editorjs" 
            className="prose lg:prose-xl max-w-none [&_h1]:text-4xl [&_h2]:text-3xl [&_h3]:text-2xl [&_h4]:text-xl"
        />
    );
};

export default Editor; 