using AutoMapper;
using InfertilityTreatment.Business.Helpers;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<string> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
       ?? throw new KeyNotFoundException("User not found.");
            var isCurrentPasswordCorrect = PasswordHelper.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordCorrect)
                throw new ArgumentException("Current password is incorrect.");

            var isNewPasswordSameAsOld = PasswordHelper.VerifyPassword(changePasswordDto.NewPassword, user.PasswordHash);
            if (isNewPasswordSameAsOld)
                throw new ArgumentException("New password must be different from the current password.");


            var password = PasswordHelper.HashPassword(changePasswordDto.NewPassword);

            var result = await _userRepository.ChangePasswordAsync(userId, password);
            if (result == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return "Profile updated successfully";

        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithProfilesAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            var profileDto = _mapper.Map<UserProfileDto>(user);
            return profileDto;
        }

        public async Task<PaginatedResultDto<UserProfileDto>> GetUsersAsync(UserFilterDto filter)
        {
            var pagedResult = await _userRepository.GetUsers(filter);

            if (pagedResult == null || !pagedResult.Items.Any())
            {
                throw new KeyNotFoundException("No users found.");
            }

            var profileDtos = _mapper.Map<List<UserProfileDto>>(pagedResult.Items);

            return new PaginatedResultDto<UserProfileDto>(
                profileDtos,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
        }




        public async Task<string> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = _mapper.Map<User>(updateProfileDto);
            user.Id = userId;

            var result = await _userRepository.UpdateProfile(user);
            if (result == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return "Profile updated successfully";

        }
    }

}
