#include <sys/types.h>
#include <sys/socket.h>
#include <sys/wait.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <stdio.h>
#include <signal.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <pthread.h>
#include <string.h>
#include <dirent.h>
#include <sys/stat.h>
#include <fcntl.h>

struct cln{
	int cfd;
	struct sockaddr_in caddr;
};

struct fileNames{
	char names[1024][1024];
	int number;
};

struct users{
	char usr[1024][20];
	int number;
};

struct usersPermited{
	char name[1024];
	char usr[1024][20];
	int number;
};

struct calendarText{
	char name[1024];
	char lines[1024][256];
	int number;
};

struct fileNames files;
struct users usrs;
struct usersPermited usrsPerm;
struct calendarText calendarTxt;

//ODCZYTANIE WSZYSTKICH NAZW PLIKÓW
struct fileNames getFileNames(char dirName[]){
	struct fileNames files;
	DIR *d;
	struct dirent *dir;
	d = opendir(dirName);
	int i = 0;
	if(d){
		while((dir = readdir(d)) != NULL){
			if(dir->d_name[0] != '.'){
				strcpy(files.names[i], dir->d_name);
				//strcat(files.names[i], "\n");
				i++;
			}
		}
		closedir(d);
	}
	files.number = i;
	return files;
} 

//ODCZYTANIE UZYTKOWNIKÓW Z DOSTEPEM DO PLIKÓW
struct usersPermited getFilesPermited(char fileName[]){
	FILE *fptr;
	struct usersPermited usrPerm;
	strcpy(usrPerm.name, fileName);
	
	char fileNameDir[1024];
	int i = 0;
	printf("SCANNING PERMITED USERS\n");
	//if(fileName[strlen(fileName)-1] == '\n') fileName[strlen(fileName)-1] = 0;
	strcpy(fileNameDir, "calendarsPermissions/");
	strcat(fileNameDir, fileName);
	//printf("file name + dir: %s", fileNameDir);
	fptr = fopen(fileNameDir, "r");
	//printf("file name + dir: %s", fileNameDir);
    char line[256];

    while (fgets(line, sizeof(line), fptr)) {
		//printf("line: %s\n", line);
		strcpy(usrPerm.usr[i], line);
		//printf("USR %d: %s\n", i, usrPerm.usr[i]);
		if(usrPerm.usr[i][strlen(usrPerm.usr[i])-1] == '\n') usrPerm.usr[i][strlen(usrPerm.usr[i])-1] = 0;
		i++;
	}
	fclose(fptr);
	usrPerm.number = i;
	strcat(fileName, "\n");

	return usrPerm;

}

//ODCZYTANIE WSZYSTKICH UZYTKOWNIKÓW
struct users getUsers(){
	FILE *fptr;

	struct users usrs;
	printf("SCANNING USERS\n");
	int i = 0;
	fptr = fopen("files/us.ers","r");
    char line[256];

    while (fgets(line, sizeof(line), fptr)) {
		//printf("line: %s: ", line);
		strcpy(usrs.usr[i], line);
		//printf("USR %d: %s", i, usrs.usr[i]);
		if(usrs.usr[i][strlen(usrs.usr[i])-1] == '\n') usrs.usr[i][strlen(usrs.usr[i])-1] = 0;
		i++;
	}
	fclose(fptr);
	usrs.number = i;

	return usrs;
}

//ODCZYTANIE ZAWARTOŚCI KALENDARZA
struct calendarText getCalendarText(char calName[]){
	FILE *fptr;
	struct calendarText calTxt;
	strcpy(calTxt.name, calName);
	
	char fileNameDir[1024];
	int i = 0;
	if(calTxt.name[strlen(calTxt.name)-1] == '\n') calTxt.name[strlen(calTxt.name)-1] = 0;
	strcpy(fileNameDir, "calendars/");
	strcat(fileNameDir, calTxt.name);
	//printf("file name + dir: %s", fileNameDir);
	fptr = fopen(fileNameDir, "r");
	//printf("file name + dir: %s", fileNameDir);
    char line[256];

    while (fgets(line, sizeof(line), fptr)) {
		//printf("line: %s\n", line);
		strcpy(calTxt.lines[i], line);
		//printf("USR %d: %s\n", i, usrPerm.usr[i]);
		//if(calTxt.lines[i][strlen(calTxt.lines[i])-1] == '\n') calTxt.lines[i][strlen(calTxt.lines[i])-1] = 0;
		i++;
	}
	fclose(fptr);
	calTxt.number = i;
	strcat(calTxt.name, "\n");

	return calTxt;
}

void* cthread(void* arg){
	struct cln* c = (struct cln*)arg;
	//ZMIENNE POMOCNICZE
	char swOpt;
	int alredyPermited = 0;
	char user[20] = "";
	int found = 0;
	char calName[1024] = "";
	char readLine[1024] = "";
	char calDirName[1024] = "";
	//KONIEC ZMIENNYCH POMOCNICZYCH
	strcpy(calDirName, "");
	//ODCZYTANIE OPCJI DZIAŁANIA SERVERA
	read(c->cfd, &swOpt, 1);
	printf("swOpt: %c\n", swOpt);
	switch(swOpt){
		case 'l': //LOGIN
			usrs = getUsers();
			printf("new connection: %s\n", inet_ntoa((struct in_addr)c->caddr.sin_addr));
			read(c->cfd, user, 20);
			for(int i = 0; i < usrs.number; i++){
				if(strcmp(user, usrs.usr[i])==0){
					write(c->cfd, "Zalogowano!", 11);
					found = 1;
					break;
				}
			}
			if(found == 0) write(c->cfd, "ERROR", 6);

		break;
		case 'n': //LOAD CALENDAR NAMES AND PERMISSIONS -- NOT USED FOR NOW
			files = getFileNames("calendars");
			printf("Getting calendars (%d): %s\n", files.number, inet_ntoa((struct in_addr)c->caddr.sin_addr));
			read(c->cfd, user, 19);
			printf("SCANNING PERMISSIONS\n");
			for(int i = files.number-1; i >= 0; i--){
				usrsPerm = getFilesPermited(files.names[i]);
				for(int j = usrsPerm.number-1; j >= 0; j--){
					if(strcmp(user, usrsPerm.usr[j]) == 0){
						write(c->cfd, files.names[i], strlen(files.names[i]));
					}
				}
			}
		break;
		case 'c': //LOAD CALENDARS
			files = getFileNames("calendars");
			printf("LOADING CALENDARS...\n");
			read(c->cfd, user, 19);
			for(int i = files.number-1; i >=0; i--){
				usrsPerm = getFilesPermited(files.names[i]);
				for(int j = usrsPerm.number-1; j >= 0; j--){
					if(strcmp(user, usrsPerm.usr[j]) == 0){
						strcpy(calName, files.names[i]);
						strcat(calName, "\n");
						calendarTxt = getCalendarText(files.names[i]);
						write(c->cfd, calName, strlen(calName));
						for(int x = 0; x < calendarTxt.number; x++){
							write(c->cfd, calendarTxt.lines[x], strlen(calendarTxt.lines[x]));
						}
						write(c->cfd, "!@#\n", 4);
					}
				}
			}

		break;
		case 'u': //UPDATE CALLENDAR
			read(c->cfd, calName, 19);
			files = getFileNames("calendars");
			printf("UPDATING CALENDARS...\n");
			printf("KALENDARZ: %s\n", calName);
			if(calName[strlen(calName)-1] == '\n') calName[strlen(calName)-1] = 0;
			for(int i = files.number-1; i >=0; i--){
				if(strcmp(files.names[i], calName) == 0) {
					found = 1;
					break;
				}
			}
			write(c->cfd, "got", 3);
			//SPRAWDZENIE CZY PODANY KALENDARZ JEST W PLIKACH
			if(found == 1) {
				strcat(calDirName, "calendars/");
				strcat(calDirName, calName);
				calendarTxt = getCalendarText(calName);
				int fptr1;
				int fptr2;
				fptr1 = remove(calDirName);
				close(fptr1);
				fptr2 = creat(calDirName, 0777);
				strcpy(readLine, "");
				int size;
				while((size = read(c->cfd, readLine, 1024))>=0){
					write(fptr2, readLine, strlen(readLine));
					//printf("LINIA: %s", readLine);
					//printf("Ilosc: %d\n", size);
					strcpy(readLine, "");
				}
				//write(c->cfd, &size, 3);
				//printf("Lghdsuihfisi");
				//write(c->cfd, "got", 3);
				close(fptr2);
			}
		break;
		case 'a'://ADD NEW CALLENDAR
			read(c->cfd, calName, 20);
			write(c->cfd, "got", 3);
			read(c->cfd, user, 20);
			printf("ADDING CALENDARS...\n");
			files = getFileNames("calendars");
			if(calName[strlen(calName)-1] == '\n') calName[strlen(calName)-1] = 0;
			

			for(int i = files.number-1; i >=0; i--){
				if(strcmp(files.names[i], calName) == 0) {
					found = 1;
					break;
				}
			}

			//SPRAWDZENIE CZY NA PEWNO NIE MA TAKIEGO KALENDARZA
			if(found == 0) {
				int fd;
				strcat(calDirName, "calendars/");
				strcat(calDirName, calName);
				fd = creat(calDirName, 0777);
				close(fd);
				strcpy(calDirName, "");
				strcat(calDirName, "calendarsPermissions/");
				strcat(calDirName, calName);
				fd = creat(calDirName, 0777);
				strcat(user, "\n");
				write(fd, user, strlen(user));
				close(fd);
			}

		break;
		case 'd'://DELETE CALLENDAR
			read(c->cfd, calName, 20);
			files = getFileNames("calendars");
			printf("DELETING CALENDARS...\n");
			if(calName[strlen(calName)-1] == '\n') calName[strlen(calName)-1] = 0;


			for(int i = files.number-1; i >=0; i--){
				if(strcmp(files.names[i], calName) == 0) {
					found = 1;
					break;
				}
			}

			//SPRAWDZENIE CZY NA PEWNO JEST WYBRANY DO USUNIECIA KALENDARZ
			if(found == 1) {
				strcat(calDirName, "calendars/");
				strcat(calDirName, calName);
				remove(calDirName);
				strcpy(calDirName, "");
				strcat(calDirName, "calendarsPermissions/");
				strcat(calDirName, calName);
				printf("calDirName: %s\n", calDirName);
				remove(calDirName);
			}
		break;
		case 'p'://ADD NEW PARTICIPANTS
			read(c->cfd, calName, 20);
			write(c->cfd, "got", 3);
			read(c->cfd, user, 20);
			printf("ADDING PARTICIPANTS TO CALENDARS...\n");


			files = getFileNames("calendarsPermissions");
			usrs = getUsers();
			usrsPerm = getFilesPermited(calName);
			if(calName[strlen(calName)-1] == '\n') calName[strlen(calName)-1] = 0;
			for(int i = 0; i < files.number; i++){
				if(files.names[i][strlen(files.names[i])-1] == '\n') files.names[i][strlen(files.names[i])-1] = 0;
				if(strcmp(files.names[i], calName) == 0) {
					found = 1;
					break;
				}
			}
			strcpy(readLine, "Nie dodano nowego usera\n");
			if(found == 1) {
				for(int i = 0; i < usrs.number; i++){
					if(strcmp(user, usrs.usr[i]) == 0){
					for(int j = 0; j < usrsPerm.number; j++){
						if(strcmp(user, usrsPerm.usr[j]) == 0){
							alredyPermited = 1;
							break;
						}
					}
					}
				}
				if(alredyPermited == 0){

					strcat(calDirName, "calendarsPermissions/");
					strcat(calDirName, calName);
					strcat(user, "\n");
				
					int fptr;
					fptr = open(calDirName, O_APPEND | O_WRONLY , 0777);
					strcpy(readLine, "Dodano nowego usera\n");
					//write(fptr, "\n", 1);
					write(fptr, user, strlen(user));
					close(fptr);
				}
			}

			printf("%s", readLine);

		break;
		default:
			printf("NOT EXISTING OPTION\n");
	}

	close(c->cfd);
	free(c);
	return EXIT_SUCCESS;
}

int main(int argc, char** argv){
	pthread_t tid;
	socklen_t slt;
	int fd, on=1;
	struct sockaddr_in saddr;

	
	fd = socket(AF_INET, SOCK_STREAM, 0);
	saddr.sin_family = AF_INET;
	saddr.sin_addr.s_addr = INADDR_ANY;
	saddr.sin_port = htons(1234);
	setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, (char*)&on, sizeof(on)); //zwalnia nr portu
	bind(fd, (struct sockaddr*)&saddr, sizeof(saddr));
	listen(fd, 5);
	while(1){
		struct cln* c = malloc(sizeof(struct cln));
		slt = sizeof(c->caddr);
		c->cfd = accept(fd, (struct sockaddr*)&c->caddr, &slt);
		pthread_create(&tid, NULL, cthread, c);
		pthread_detach(tid);
	}
	close(fd);
	return EXIT_SUCCESS;
}
