import Header from '@editorjs/header';
import Paragraph from '@editorjs/paragraph';
import List from '@editorjs/list';
import Image from '@editorjs/image';
import { mediaService } from '../../services/mediaService';

export const EDITOR_JS_TOOLS = {
    header: {
        class: Header,
        config: {
            placeholder: 'Enter a header',
            levels: [1, 2, 3, 4],
            defaultLevel: 2
        }
    },
    paragraph: {
        class: Paragraph,
        inlineToolbar: ['bold', 'italic', 'link'],
    },
    list: {
        class: List,
        inlineToolbar: ['bold', 'italic', 'link'],
    },
    image: {
        class: Image,
        config: {
            uploader: {
                uploadByFile(file) {
                    return new Promise((resolve, reject) => {
                        const formData = new FormData();
                        formData.append('file', file);
                        formData.append('isAvatar', false);
                        
                        mediaService.loadMedia(formData)
                            .then(response => {
                                resolve({
                                    success: 1,
                                    file: {
                                        url: mediaService.makeImageUrl(response.data.id)
                                    }
                                });
                            })
                            .catch(error => {
                                reject(error);
                            });
                    });
                }
            }
        }
    }
};

export const EDITOR_JS_CONFIG = {
    tools: EDITOR_JS_TOOLS,
    inlineToolbar: ['bold', 'italic', 'link'],
    placeholder: 'Start writing your article...',
    data: {
        blocks: [
            {
                type: "paragraph",
                data: {
                    text: "Start writing your article..."
                }
            }
        ]
    }
}; 